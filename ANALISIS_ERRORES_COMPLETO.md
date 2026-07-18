# Análisis Profundo - Especificación de Errores y Correcciones
## SistemaPOS.GVG

---

## 1️⃣ ERROR CRÍTICO: JsonException en Login.xaml.cs

### Problema Identificado
```
Exception: System.Text.Json.JsonException
Message: A possible object cycle was detected...
Path: $.Password.SelectionBrush.Transform.Inverse.Inverse...
```

### Ubicación
- **Archivo:** `SistemaPOS.GVG\Login.xaml.cs`
- **Método:** `btnIngresar_Click` (línea 32)
- **Línea problemática:** `var loginData = new { Username = usuario, Password = txtPassword };`

### Raíz del Problema
Se está intentando serializar **el control WPF completo (PasswordBox)** en lugar del **valor de texto**.

El `PasswordBox` contiene propiedades complejas:
```
PasswordBox
└── SelectionBrush (Brush)
	└── Transform (Transform)
		└── Inverse (Transform) ← Propiedad cíclica
			└── Inverse
				└── Inverse
					└── ... [hasta exceder 64 niveles]
```

### Solución
**Cambiar línea 32 de:**
```csharp
var loginData = new { Username = usuario, Password = txtPassword };
```

**A:**
```csharp
var loginData = new { Username = usuario, Password = password };
```

**Justificación:** La variable `password` ya contiene el valor extraído correctamente en la línea 21:
```csharp
string password = txtPassword.Password;
```

---

## 2️⃣ ERRORES POTENCIALES EN ARQUITECTURA

### A. En ApiClient.cs (Servicio de Cliente HTTP)

**Riesgo:** La configuración de `JsonSerializer` podría no estar especificada.

**Síntoma:** 
- Si se reutiliza en múltiples lugares, propaga el problema
- Propiedades de navegación de entidades podrían causar ciclos

**Solución Recomendada:**
```csharp
var options = new JsonSerializerOptions
{
	PropertyNameCaseInsensitive = true,
	ReferenceHandler = ReferenceHandler.Preserve,  // Para manejar ciclos
	MaxDepth = 64
};

var json = JsonSerializer.Serialize(loginData, options);
```

### B. En Modelos de Entity Framework

**Riesgo:** Propiedades de navegación sin `[JsonIgnore]` causan ciclos.

**Archivos Afectados Potenciales:**
- `SistemaPOS.GVG.API\Models\Usuario.cs`
- `SistemaPOS.GVG.API\Models\Venta.cs`
- `SistemaPOS.GVG.API\Models\Cliente.cs`
- `SistemaPOS.GVG.API\Models\DetalleVenta.cs`
- `SistemaPOS.GVG.API\Models\Producto.cs`

**Ejemplo de Problema:**
```csharp
public class Venta
{
	public int Id { get; set; }
	public int ClienteId { get; set; }

	// ❌ PROBLEMA: Esto causa ciclo al serializar
	public virtual Cliente Cliente { get; set; }

	// ❌ PROBLEMA: Esto causa ciclo al serializar
	public virtual ICollection<DetalleVenta> DetallesVenta { get; set; }
}
```

**Solución:**
```csharp
[JsonIgnore]
public virtual Cliente Cliente { get; set; }

[JsonIgnore]
public virtual ICollection<DetalleVenta> DetallesVenta { get; set; }
```

### C. En Program.cs (Configuración de API)

**Riesgo:** No se configura globalmente `JsonSerializerOptions`.

**Solución Recomendada:**
```csharp
builder.Services.Configure<JsonOptions>(options =>
{
	options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
	options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
	options.JsonSerializerOptions.MaxDepth = 64;
	options.JsonSerializerOptions.DefaultIgnoreCondition = 
		System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});
```

### D. En AuthController.cs

**Riesgo:** Validación incorrecta de `LoginDTO`.

**Verificaciones Necesarias:**
- ¿Se valida que `LoginDTO` sea no nulo?
- ¿Se valida que Username y Password no estén vacíos?
- ¿Se valida longitud máxima?
- ¿Se valida formato de datos?

**Ejemplo de Validación Robusta:**
```csharp
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
{
	// Validación de entrada
	if (loginDto == null)
	{
		return BadRequest(new { message = "Datos de login requeridos" });
	}

	if (string.IsNullOrWhiteSpace(loginDto.Username) || 
		string.IsNullOrWhiteSpace(loginDto.Password))
	{
		return BadRequest(new { message = "Usuario y contraseña son requeridos" });
	}

	// Lógica de autenticación...
}
```

### E. En Vistas (Potencial Patrón Repetido)

**Riesgo:** Otras vistas podrían tener el mismo problema de serialización.

**Archivos a Revisar:**
- `VentasView.xaml.cs` - ¿Serializa correctamente objetos?
- `ProductosView.xaml.cs` - ¿Maneja DTOs correctamente?
- `ClientesView.xaml.cs` - ¿Serializa datos de formularios?
- Todos los `.xaml.cs` en la carpeta `Views\`

---

## 3️⃣ ERRORES POR MANEJO DE EXCEPCIONES

### Problema: catch genérico en Login.xaml.cs

**Código actual:**
```csharp
catch (HttpRequestException ex)
{
	MessageBox.Show("No se pudo conectar...");
}
```

**Problema:** Solo captura `HttpRequestException`, pero:
- `JsonException` NO es capturada ← **Esto es el error actual**
- `TaskCanceledException` NO es capturada
- Otras excepciones no se manejan

**Solución Mejorada:**
```csharp
catch (JsonException ex)
{
	MessageBox.Show(
		"Error al serializar datos. Verifique el formato de entrada.",
		"Error de Serialización",
		MessageBoxButton.OK,
		MessageBoxImage.Error);
	Debug.WriteLine($"JsonException: {ex.Message}");
}
catch (HttpRequestException ex)
{
	MessageBox.Show(
		"No se pudo conectar con el servidor. Verifique que la API esté en ejecución.",
		"Error de Conexión",
		MessageBoxButton.OK,
		MessageBoxImage.Error);
}
catch (TaskCanceledException ex)
{
	MessageBox.Show(
		"La solicitud tardó demasiado. Intente nuevamente.",
		"Timeout",
		MessageBoxButton.OK,
		MessageBoxImage.Error);
}
catch (Exception ex)
{
	MessageBox.Show(
		$"Error inesperado: {ex.Message}",
		"Error",
		MessageBoxButton.OK,
		MessageBoxImage.Error);
}
```

---

## 4️⃣ ERRORES POR CONFIGURACIÓN DE HTTPCLIENT

### Problema: HttpClient instanciado en cada petición

**Código actual:**
```csharp
using (var client = new HttpClient())
{
	client.BaseAddress = new Uri("https://localhost:7269");
	// ... petición
}
```

**Problemas:**
- ❌ Ineficiente (crea nueva conexión cada vez)
- ❌ Socket exhaustion en aplicaciones de larga duración
- ❌ No reutiliza conexiones
- ❌ No valida certificados SSL correctamente

**Solución:**
```csharp
private static readonly HttpClient _httpClient = new HttpClient()
{
	BaseAddress = new Uri("https://localhost:7269"),
	Timeout = TimeSpan.FromSeconds(30)
};

// En el método:
try
{
	var response = await _httpClient.PostAsync("api/auth/login", content);
	// ...
}
```

O mejor aún, usar `HttpClientFactory` (recomendado):

```csharp
// En Program.cs:
services.AddHttpClient<IApiClient, ApiClient>(client =>
{
	client.BaseAddress = new Uri("https://localhost:7269");
	client.Timeout = TimeSpan.FromSeconds(30);
});
```

---

## 5️⃣ ERRORES POR VALIDACIÓN DE CERTIFICADOS SSL

### Problema: Conexión a `https://localhost:7269` en desarrollo

**Síntoma:** Posible error `HttpRequestException` debido a certificado no válido.

**Solución para Desarrollo (NO PRODUCCIÓN):**
```csharp
// SOLO PARA DESARROLLO
var handler = new HttpClientHandler();
handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
{
	// Solo en desarrollo/testing
	if (message.RequestUri?.Host == "localhost")
		return true;

	return errors == System.Net.Security.SslPolicyErrors.None;
};

using (var client = new HttpClient(handler))
{
	// ...
}
```

---

## 6️⃣ MATRIZ DE RIESGOS

| Riesgo | Severidad | Probabilidad | Impacto |
|--------|-----------|--------------|---------|
| JsonException por PasswordBox | 🔴 Crítico | Alto | Fallo de login |
| Ciclos en modelos EF | 🔴 Crítico | Medio | Excepciones al obtener datos |
| Configuración JsonOptions incorrecta | 🟡 Alto | Medio | Problemas de serialización |
| HttpClient mal configurado | 🟡 Alto | Bajo | Socket exhaustion |
| Catch genérico insuficiente | 🟡 Alto | Medio | Errores no manejados |
| Validación faltante en API | 🟡 Alto | Medio | Inyección de datos |

---

## 7️⃣ PLAN DE CORRECCIONES (ORDEN DE PRIORIDAD)

### ✅ PRIORIDAD 1 (Crítico - Arreglar YA)
1. **Corregir Login.xaml.cs línea 32** - Cambiar `txtPassword` a `password`
2. **Agregar [JsonIgnore]** en modelos EF de navegación
3. **Mejorar manejo de excepciones** en Login.xaml.cs

### 🟡 PRIORIDAD 2 (Alto - Arreglar esta sesión)
4. **Configurar JsonSerializerOptions** en ApiClient
5. **Configurar Program.cs** con opciones JSON globales
6. **Validar AuthController.cs** para validaciones de entrada

### 🟢 PRIORIDAD 3 (Medio - Arreglar después)
7. **Reemplazar HttpClient** por singleton o HttpClientFactory
8. **Validar certificados SSL** apropiadamente
9. **Revisar todas las vistas** para patrones similares

---

## 📝 RECOMENDACIONES FINALES

### Inmediato
✓ Aplicar corrección de línea 32 en Login.xaml.cs
✓ Crear `JsonSerializerOptionsHelper.cs`
✓ Revisar modelos EF para propiedades de navegación

### Corto Plazo
✓ Configurar correctamente `Program.cs`
✓ Mejorar manejo de excepciones
✓ Agregar validaciones en controladores API

### Largo Plazo
✓ Implementar logging centralizado
✓ Agregar tests unitarios
✓ Documentar API con OpenAPI/Swagger
✓ Implementar rate limiting en API

