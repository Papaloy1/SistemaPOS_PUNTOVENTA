using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPOS.API.Data;
using SistemaPOS.API.Models;

namespace SistemaPOS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<VentasController> _logger;

        public VentasController(AppDbContext context, ILogger<VentasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/ventas
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Venta>>>> GetVentas(int? dias = null)
        {
            try
            {
                _logger.LogInformation("Consultando ventas");

                IQueryable<Venta> query = _context.Ventas
                    .Include(v => v.Cliente)
                    .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto);

                if (dias.HasValue)
                {
                    var fechaLimite = DateTime.Now.AddDays(-dias.Value);
                    query = query.Where(v => v.FechaVenta >= fechaLimite);
                }

                var ventas = await query.OrderByDescending(v => v.FechaVenta).ToListAsync();
                return Ok(ApiResponse<IEnumerable<Venta>>.SuccessResponse(ventas,
                    $"Se encontraron {ventas.Count} ventas"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar ventas");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Error al consultar ventas",
                    new List<string> { ex.Message }));
            }
        }

        // GET: api/ventas/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Venta>>> GetVenta(int id)
        {
            try
            {
                var venta = await _context.Ventas
                    .Include(v => v.Cliente)
                    .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto)
                    .FirstOrDefaultAsync(v => v.IdVenta == id);

                if (venta == null)
                    return NotFound(ApiResponse<object>.ErrorResponse($"Venta con ID {id} no encontrada"));

                return Ok(ApiResponse<Venta>.SuccessResponse(venta, "Venta encontrada"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al consultar venta ID: {id}");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Error al consultar venta",
                    new List<string> { ex.Message }));
            }
        }

        // POST: api/ventas
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Venta>>> PostVenta(Venta venta)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Datos inválidos",
                        ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));

                // Validar detalles
                if (!venta.Detalles.Any())
                    return BadRequest(ApiResponse<object>.ErrorResponse("La venta debe incluir al menos un detalle"));

                // Calcular totales
                venta.Subtotal = venta.Detalles.Sum(d => d.Subtotal);
                venta.Total = venta.Subtotal + venta.Impuesto;
                venta.Cambio = venta.Pagado - venta.Total;

                // Actualizar stock
                foreach (var detalle in venta.Detalles)
                {
                    var producto = await _context.Productos.FindAsync(detalle.IdProducto);
                    if (producto != null)
                    {
                        producto.Stock -= detalle.Cantidad;
                    }
                }

                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Venta creada: {venta.IdVenta}");
                return CreatedAtAction(nameof(GetVenta), new { id = venta.IdVenta },
                    ApiResponse<Venta>.SuccessResponse(venta, "Venta registrada exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear venta");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Error al crear venta",
                    new List<string> { ex.Message }));
            }
        }

        // GET: api/ventas/reporte/resumen
        [HttpGet("reporte/resumen")]
        public async Task<ActionResult<ApiResponse<object>>> GetResumeVentas(int? dias = 30)
        {
            try
            {
                var fechaLimite = DateTime.Now.AddDays(-(dias ?? 30));
                var ventas = await _context.Ventas
                    .Where(v => v.FechaVenta >= fechaLimite && !v.Cancelada)
                    .ToListAsync();

                var resumen = new
                {
                    TotalVentas = ventas.Count,
                    MontoTotal = ventas.Sum(v => v.Total),
                    Promedio = ventas.Any() ? ventas.Average(v => v.Total) : 0,
                    VentasPorTipoPago = ventas.GroupBy(v => v.TipoPago)
                        .Select(g => new { TipoPago = g.Key, Cantidad = g.Count(), Monto = g.Sum(v => v.Total) })
                        .ToList()
                };

                return Ok(ApiResponse<object>.SuccessResponse(resumen, "Resumen de ventas"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar resumen de ventas");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Error al generar resumen",
                    new List<string> { ex.Message }));
            }
        }
    }
}
