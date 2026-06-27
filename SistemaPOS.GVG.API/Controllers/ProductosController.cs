using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPOS.API.Data;
using SistemaPOS.API.Models;
using System.Collections.Generic;

namespace SistemaPOS.API.Controllers
{
    // Esta ruta define que el endpoint será: http://localhost:puerto/api/productos
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductosController> _logger;

        // Inyección de dependencias del DbContext
        public ProductosController(AppDbContext context, ILogger<ProductosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Productos
        // Retorna el catálogo completo de pinturas y consumibles
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Producto>>>> GetProductos()
        {
            try
            {
                _logger.LogInformation("Consultando todos los productos");
                var productos = await _context.Productos.ToListAsync();
                return Ok(ApiResponse<IEnumerable<Producto>>.SuccessResponse(productos, 
                    $"Se encontraron {productos.Count} productos"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar productos");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Error al consultar productos", 
                    new List<string> { ex.Message }));
            }
        }

        // GET: api/Productos/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Producto>>> GetProducto(int id)
        {
            try
            {
                _logger.LogInformation($"Consultando producto con ID: {id}");
                var producto = await _context.Productos.FindAsync(id);

                if (producto == null)
                {
                    _logger.LogWarning($"Producto con ID {id} no encontrado");
                    return NotFound(ApiResponse<object>.ErrorResponse($"Producto con ID {id} no encontrado"));
                }

                return Ok(ApiResponse<Producto>.SuccessResponse(producto, "Producto encontrado"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al consultar producto ID: {id}");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Error al consultar producto", 
                    new List<string> { ex.Message }));
            }
        }

        // GET: api/Productos/buscar/codigo/{codigoBarras}
        [HttpGet("buscar/codigo/{codigoBarras}")]
        public async Task<ActionResult<ApiResponse<Producto>>> GetProductoPorCodigoBarras(string codigoBarras)
        {
            try
            {
                _logger.LogInformation($"Buscando producto por código de barras: {codigoBarras}");
                var producto = await _context.Productos
                    .FirstOrDefaultAsync(p => p.CodigoBarras == codigoBarras);

                if (producto == null)
                {
                    _logger.LogWarning($"Producto con código {codigoBarras} no encontrado");
                    return NotFound(ApiResponse<object>.ErrorResponse(
                        $"Producto con código de barras '{codigoBarras}' no encontrado"));
                }

                return Ok(ApiResponse<Producto>.SuccessResponse(producto, "Producto encontrado"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al buscar producto por código: {codigoBarras}");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Error al buscar producto", 
                    new List<string> { ex.Message }));
            }
        }

        // POST: api/Productos
        // Permite registrar un nuevo producto en el sistema
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Producto>>> PostProducto(Producto producto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Intento de crear producto con datos inválidos");
                    return BadRequest(ApiResponse<object>.ErrorResponse("Datos inválidos", errors));
                }

                // Verificar si ya existe un producto con el mismo código de barras
                var existente = await _context.Productos
                    .FirstOrDefaultAsync(p => p.CodigoBarras == producto.CodigoBarras);
                
                if (existente != null)
                {
                    _logger.LogWarning($"Intento de crear producto con código duplicado: {producto.CodigoBarras}");
                    return Conflict(ApiResponse<object>.ErrorResponse(
                        $"Ya existe un producto con el código de barras '{producto.CodigoBarras}'"));
                }

                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Producto creado exitosamente: {producto.IdProducto}");
                return CreatedAtAction(nameof(GetProducto), new { id = producto.IdProducto }, 
                    ApiResponse<Producto>.SuccessResponse(producto, "Producto creado exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Error al crear producto", 
                    new List<string> { ex.Message }));
            }
        }

        // PUT: api/Productos/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Producto>>> PutProducto(int id, Producto producto)
        {
            try
            {
                if (id != producto.IdProducto)
                {
                    _logger.LogWarning($"ID mismatch: {id} != {producto.IdProducto}");
                    return BadRequest(ApiResponse<object>.ErrorResponse("El ID en la URL no coincide con el producto"));
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage).ToList();
                    return BadRequest(ApiResponse<object>.ErrorResponse("Datos inválidos", errors));
                }

                _context.Entry(producto).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Producto actualizado exitosamente: {id}");
                return Ok(ApiResponse<Producto>.SuccessResponse(producto, "Producto actualizado exitosamente"));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ProductoExists(id))
                {
                    _logger.LogWarning($"Producto {id} no encontrado durante actualización");
                    return NotFound(ApiResponse<object>.ErrorResponse($"Producto con ID {id} no encontrado"));
                }
                _logger.LogError(ex, "Error de concurrencia al actualizar producto");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Error de concurrencia al actualizar", 
                    new List<string> { ex.Message }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar producto ID: {id}");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Error al actualizar producto", 
                    new List<string> { ex.Message }));
            }
        }

        // DELETE: api/Productos/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteProducto(int id)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(id);
                if (producto == null)
                {
                    _logger.LogWarning($"Intento de eliminar producto {id} no encontrado");
                    return NotFound(ApiResponse<object>.ErrorResponse($"Producto con ID {id} no encontrado"));
                }

                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Producto eliminado exitosamente: {id}");
                return Ok(ApiResponse<object>.SuccessResponse(null, "Producto eliminado exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar producto ID: {id}");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Error al eliminar producto", 
                    new List<string> { ex.Message }));
            }
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.IdProducto == id);
        }
    }
}