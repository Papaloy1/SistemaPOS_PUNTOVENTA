# Análisis Profundo Proyecto SistemaPOS.GVG

## 1. ERRORES CRÍTICOS IDENTIFICADOS

### ❌ ERROR 1: PasswordBox Serialization (Login.xaml.cs:32)
- **Ubicación:** `SistemaPOS.GVG\Login.xaml.cs`
- **Línea:** 32
- **Problema:** `var loginData = new { Username = usuario, Password = txtPassword };`
- **Causa:** Intenta serializar control WPF completo en lugar de valor
- **Solución:** Cambiar a `var loginData = new { Username = usuario, Password = password };`

## 2. PATRONES A VERIFICAR EN OTROS ARCHIVOS

### 2.1 En Vistas (Views/*.xaml.cs)
- [ ] ¿Hay más instancias de PasswordBox siendo serializados directamente?
- [ ] ¿Se usan correctamente async/await en eventos UI?
- [ ] ¿Se manejan excepciones correctamente?

### 2.2 En Servicios (Services/*.cs)
- [ ] ¿ApiClient configura correctamente JsonSerializerOptions?
- [ ] ¿Hay referencias cíclicas en modelos?
- [ ] ¿Se valida entrada de datos?

### 2.3 En API (Controllers/*.cs)
- [ ] ¿LoginDTO coincide con datos enviados desde cliente?
- [ ] ¿Hay validaciones de datos?
- [ ] ¿Se manejan errores correctamente?

### 2.4 En Modelos (Models/*.cs)
- [ ] ¿Hay propiedades de navegación que causen ciclos?
- [ ] ¿Se configuran correctamente en Entity Framework?
- [ ] ¿Falta [JsonIgnore] en propiedades relacionales?

### 2.5 En Configuración (Program.cs)
- [ ] ¿Se configura JsonSerializerOptions globalmente?
- [ ] ¿Se habilita CORS correctamente?
- [ ] ¿Se configura Entity Framework correctamente?

## 3. LISTA DE ARCHIVOS A REVISAR

### Vistas:
- [ ] SistemaPOS.GVG\Login.xaml.cs
- [ ] SistemaPOS.GVG\DashboardWindow.xaml.cs
- [ ] SistemaPOS.GVG\Views\VentasView.xaml.cs
- [ ] SistemaPOS.GVG\Views\ProductosView.xaml.cs
- [ ] SistemaPOS.GVG\Views\ClientesView.xaml.cs
- [ ] SistemaPOS.GVG\Views\CajaView.xaml.cs
- [ ] SistemaPOS.GVG\Views\InicioView.xaml.cs
- [ ] SistemaPOS.GVG\Views\InventarioView.xaml.cs
- [ ] SistemaPOS.GVG\Views\NuevoProductoDialog.xaml.cs

### Servicios:
- [ ] SistemaPOS.GVG\Services\ApiClient.cs
- [ ] SistemaPOS.GVG\Services\TicketPrinterService.cs

### Controladores API:
- [ ] SistemaPOS.GVG.API\Controllers\AuthController.cs
- [ ] SistemaPOS.GVG.API\Controllers\ProductosController.cs
- [ ] SistemaPOS.GVG.API\Controllers\ClientesController.cs
- [ ] SistemaPOS.GVG.API\Controllers\VentasController.cs
- [ ] SistemaPOS.GVG.API\Controllers\CajasController.cs

### Modelos API:
- [ ] SistemaPOS.GVG.API\Models\AppDbContext.cs
- [ ] SistemaPOS.GVG.API\Models\Usuario.cs
- [ ] SistemaPOS.GVG.API\Models\Producto.cs
- [ ] SistemaPOS.GVG.API\Models\Cliente.cs
- [ ] SistemaPOS.GVG.API\Models\Venta.cs
- [ ] SistemaPOS.GVG.API\Models\DetalleVenta.cs
- [ ] SistemaPOS.GVG.API\Models\Caja.cs
- [ ] SistemaPOS.GVG.API\Models\LoginDTO.cs

### DTOs Cliente:
- [ ] SistemaPOS.GVG\Models\CajaDTO.cs
- [ ] SistemaPOS.GVG\Models\ClienteDTO.cs
- [ ] SistemaPOS.GVG\Models\ProductoDTO.cs
- [ ] SistemaPOS.GVG\Models\VentaDTO.cs
- [ ] SistemaPOS.GVG\Models\DetalleVentaDTO.cs
- [ ] SistemaPOS.GVG\Models\ApiResponse.cs

### Configuración:
- [ ] SistemaPOS.GVG.API\Program.cs
- [ ] SistemaPOS.GVG.API\appsettings.json

