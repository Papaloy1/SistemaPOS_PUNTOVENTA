# 🔧 SOLUCIÓN: Error 500 en Endpoint de Productos

## 🔍 DIAGNÓSTICO DEL PROBLEMA

### Error Reportado
```
Error de red al consultar el inventario
Endpoint: 'Productos'
Response status code 500 (Internal Server Error)
```

### Causa Identificada
La **conexión a la base de datos está fallando**. La razón:

```
Servidor configurado:  PCMaster
Problema:              ❌ Este servidor no existe en tu máquina
Solución:              ✅ Cambiar a (local) o localhost
```

---

## ✅ SOLUCIÓN PASO A PASO

### PASO 1: Verificar SQL Server Instalado

Abre PowerShell y ejecuta:

```powershell
# Listar instancias de SQL Server
Get-Service | Where-Object {$_.Name -like "*SQL*"}

# Resultado esperado:
# MSSQLSERVER - SQL Server (MSSQLSERVER)
```

Si ves `MSSQLSERVER`, está instalado. Si no, necesitas instalar SQL Server Express.

---

### PASO 2: Actualizar Cadena de Conexión

**Archivo a editar:** `SistemaPOS.GVG.API\appsettings.json`

#### Opción A: SQL Server Local (RECOMENDADO)
```json
"DefaultConnection": "Server=(local);Database=PuntosDeVentaDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

#### Opción B: Si tienes SQL Server en red
```json
"DefaultConnection": "Server=tu_servidor;Database=PuntosDeVentaDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

#### Opción C: Con usuario y contraseña
```json
"DefaultConnection": "Server=(local);Database=PuntosDeVentaDB;User Id=sa;Password=tu_contraseña;TrustServerCertificate=True;"
```

---

### PASO 3: Crear Base de Datos y Aplicar Migraciones

Abre PowerShell **como Administrador** y ejecuta:

```powershell
# Navegar a la carpeta de la API
cd "C:\Users\bryan\source\repos\SistemaPOS_PUNTOVENTA\SistemaPOS.GVG.API"

# Crear base de datos e aplicar migraciones
dotnet ef database update

# Resultado esperado:
# Build started...
# Build succeeded.
# Applying migration '20260618185714_MigracionInicial'.
# Applying migration '20250101000000_AgregarStock'.
# Done.
```

---

### PASO 4: Reiniciar API

1. Visual Studio → Detener la API (Shift+F5)
2. Esperar 3 segundos
3. Iniciar la API nuevamente (F5)

---

### PASO 5: Probar de Nuevo

Intenta hacer una consulta en Swagger:
- URL: `http://localhost:5275/swagger`
- Endpoint: `GET /api/productos`

**Resultado esperado:** ✅ 200 OK (lista vacía o con productos)

---

## 🆘 SI AÚN FALLA

### Verificar logs en Visual Studio

1. Visual Studio → Debug → Windows → Output
2. Buscar mensajes de error sobre conexión
3. Errores típicos:
   - `Unable to connect to server`
   - `Login failed for user`
   - `Database does not exist`

### Solucionar según el error

#### ❌ "Unable to connect to server"
```
Causa: SQL Server no está corriendo
Solución: 
  1. Services → SQL Server (MSSQLSERVER)
  2. Right-click → Start
```

#### ❌ "Login failed"
```
Causa: Usuario/contraseña incorrectos
Solución: Cambiar de Trusted_Connection=True
```

#### ❌ "Database does not exist"
```
Causa: Migración no se aplicó
Solución: dotnet ef database update (ver PASO 3)
```

---

## 🧪 VERIFICACIÓN MANUAL

### Verificar conexión SQL Server

```powershell
# Instalar herramienta (si no la tienes)
dotnet tool install -g sqlcmdline

# Conectar a SQL Server
sqlcmd -S (local)

# Si ves "1>" es que conectó exitosamente
# Salir con: exit
```

### Crear base de datos manualmente (alternativa)

```sql
CREATE DATABASE PuntosDeVentaDB;
GO
```

---

## 📝 CHECKLIST DE SOLUCIÓN

- [ ] Verificar que SQL Server esté corriendo
- [ ] Actualizar cadena de conexión a `(local)`
- [ ] Ejecutar `dotnet ef database update`
- [ ] Reiniciar la API
- [ ] Probar en Swagger

---

## 🎯 RESULTADO ESPERADO

Si todo funciona correctamente:

```
API iniciada en: http://localhost:5275
Swagger disponible en: http://localhost:5275/swagger

GET /api/productos → ✅ 200 OK
```

---

## 📞 REFERENCIAS

- [SQL Server Express Download](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Entity Framework Core - Migrations](https://docs.microsoft.com/ef/core/managing-schemas/migrations/)
- [Connection Strings Reference](https://docs.microsoft.com/dotnet/framework/data/adonet/connection-strings)

---

**Si aún tienes problemas, verifica los logs en Visual Studio Output window.**
