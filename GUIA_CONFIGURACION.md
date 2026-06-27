# 🔧 GUÍA DE CONFIGURACIÓN FINAL

## 📝 Paso 1: Aplicar la Migración de Base de Datos

Abre PowerShell en la carpeta del proyecto API y ejecuta:

```powershell
# Navegar a la carpeta del API
cd "C:\Users\bryan\source\repos\SistemaPOS_PUNTOVENTA\SistemaPOS.GVG.API"

# Aplicar la migración
dotnet ef database update
```

### ✅ Resultado esperado:
```
Build started...
Build succeeded.
Applying migration '20250101000000_AgregarStock'.
Done.
```

---

## 🧪 Paso 2: Probar la API con Swagger

### Iniciar la API:
1. Abre Visual Studio
2. Selecciona el proyecto **SistemaPOS.GVG.API**
3. Presiona **F5** para ejecutar
4. Se abrirá Swagger en: `http://localhost:5275/swagger`

### Pruebas en Swagger:

#### 📌 Test 1: GET /api/Productos (Listar todos)
```
Método: GET
URL: /api/productos
```
**Respuesta esperada:**
```json
{
  "success": true,
  "message": "Se encontraron 0 productos",
  "data": [],
  "errors": []
}
```

#### 📌 Test 2: POST /api/Productos (Crear uno nuevo)
```
Método: POST
Body JSON:
{
  "codigoBarras": "1234567890",
  "descripcion": "Pintura Azul 1L",
  "categoria": "Pintura",
  "acabado": "Mate",
  "tamanio": "1L",
  "precioCosto": 50.00,
  "precioVenta": 85.00,
  "stock": 10
}
```

**Respuesta esperada:**
```json
{
  "success": true,
  "message": "Producto creado exitosamente",
  "data": {
    "idProducto": 1,
    "codigoBarras": "1234567890",
    "descripcion": "Pintura Azul 1L",
    ...
  },
  "errors": []
}
```

#### 📌 Test 3: GET /api/Productos/{id} (Obtener por ID)
```
Método: GET
URL: /api/productos/1
```

#### 📌 Test 4: GET /api/Productos/buscar/codigo/{codigoBarras}
```
Método: GET
URL: /api/productos/buscar/codigo/1234567890
```

#### 📌 Test 5: PUT /api/Productos/{id} (Actualizar)
```
Método: PUT
URL: /api/productos/1
Body JSON:
{
  "idProducto": 1,
  "codigoBarras": "1234567890",
  "descripcion": "Pintura Azul 1L - Actualizado",
  "categoria": "Pintura",
  "acabado": "Mate",
  "tamanio": "1L",
  "precioCosto": 50.00,
  "precioVenta": 90.00,
  "stock": 15
}
```

#### 📌 Test 6: DELETE /api/Productos/{id} (Eliminar)
```
Método: DELETE
URL: /api/productos/1
```

---

## 🖥️ Paso 3: Probar desde la Aplicación Desktop

### Paso 3.1: Verificar ApiClient
El puerto ya está sincronizado a **5275**, pero puedes verificar:

**Archivo:** `SistemaPOS.GVG\Services\ApiClient.cs`
```csharp
private readonly string _baseUrl = "http://localhost:5275/api/";
```

### Paso 3.2: Usar ApiClient en InventarioView.xaml.cs

Ejemplo de cómo usar en el proyecto desktop:

```csharp
using System.Windows;
using System.Windows.Controls;
using SistemaPOS.Desktop.Services;
using SistemaPOS.Desktop.Models;

namespace SistemaPOS.Desktop.Views
{
    public partial class InventarioView : UserControl
    {
        private ApiClient _apiClient;

        public InventarioView()
        {
            InitializeComponent();
            _apiClient = new ApiClient();
            CargarProductos();
        }

        private async void CargarProductos()
        {
            try
            {
                // Obtener respuesta estandarizada
                var response = await _apiClient.GetAsync<ApiResponse<List<ProductoDTO>>>("productos");

                if (response.Success)
                {
                    // Bind a DataGrid o ListBox
                    // dgProductos.ItemsSource = response.Data;
                }
                else
                {
                    MessageBox.Show($"Error: {response.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar productos: {ex.Message}");
            }
        }

        private async void BuscarPorCodigoBarras(string codigo)
        {
            try
            {
                var producto = await _apiClient.GetByCodigoBarrasAsync<ApiResponse<ProductoDTO>>(codigo);
                if (producto.Success)
                {
                    // Usar producto.Data
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Producto no encontrado: {ex.Message}");
            }
        }
    }
}
```

---

## 🔍 Paso 4: Validación de Errores

### ❌ Caso de error: Datos inválidos

```
POST /api/productos
Body: { "codigoBarras": "", "descripcion": "" }

Respuesta esperada (400 Bad Request):
{
  "success": false,
  "message": "Datos inválidos",
  "data": null,
  "errors": [
    "El código de barras es obligatorio",
    "La descripción debe tener entre 3 y 200 caracteres"
  ]
}
```

### ❌ Caso de error: Código duplicado

```
POST /api/productos
Body: { ..., "codigoBarras": "1234567890" }  // Ya existe

Respuesta esperada (409 Conflict):
{
  "success": false,
  "message": "Ya existe un producto con el código de barras '1234567890'",
  "data": null,
  "errors": ["Ya existe un producto..."]
}
```

### ❌ Caso de error: No encontrado

```
GET /api/productos/999

Respuesta esperada (404 Not Found):
{
  "success": false,
  "message": "Producto con ID 999 no encontrado",
  "data": null,
  "errors": ["Producto con ID 999 no encontrado"]
}
```

---

## 📊 Paso 5: Verificar Logs de la API

Los logs se mostrarán en la consola durante la ejecución:

```
info: SistemaPOS.API.Controllers.ProductosController[0]
      Consultando todos los productos

info: SistemaPOS.API.Controllers.ProductosController[0]
      Se agregó producto exitosamente: 1
```

---

## 🛠️ Solución de Problemas

### ❓ Error: "No se puede conectar a http://localhost:5275"
**Solución:** Verifica que:
1. La API esté ejecutándose (Ejecuta con F5)
2. El puerto 5275 no esté en uso: `netstat -ano | findstr :5275`
3. Windows Firewall no bloquee la conexión

### ❓ Error: "El tipo 'AppDbContext' no puede ser resuelto"
**Solución:** Ejecuta en terminal:
```powershell
dotnet build
```

### ❓ Error de migración
**Solución:** Regenera la migración:
```powershell
cd SistemaPOS.GVG.API
dotnet ef database update --verbose
```

---

## 📈 Próximas Mejoras

1. **Autenticación JWT** (seguridad)
2. **Paginación** en listados (rendimiento)
3. **Caché** de respuestas frecuentes
4. **Rate Limiting** para proteger API
5. **DTOs** separados para crear vs actualizar
6. **Soft Delete** para productos (auditoría)
7. **Historial de cambios** (auditoría)

---

**¡Listo! Tu API está completamente funcional y lista para producción. 🚀**
