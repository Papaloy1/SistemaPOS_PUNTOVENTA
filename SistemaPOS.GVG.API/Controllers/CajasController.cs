using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPOS.API.Data;
using SistemaPOS.API.Models;

namespace SistemaPOS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CajasController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CajasController> _logger;

        public CajasController(AppDbContext context, ILogger<CajasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/cajas
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Caja>>>> GetCajas()
        {
            try
            {
                _logger.LogInformation("Consultando cajas");
                var cajas = await _context.Cajas.ToListAsync();
                return Ok(ApiResponse<IEnumerable<Caja>>.SuccessResponse(cajas,
                    $"Se encontraron {cajas.Count} cajas"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar cajas");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Error al consultar cajas",
                    new List<string> { ex.Message }));
            }
        }

        // GET: api/cajas/activa
        [HttpGet("activa")]
        public async Task<ActionResult<ApiResponse<Caja>>> GetCajaActiva()
        {
            try
            {
                var caja = await _context.Cajas
                    .FirstOrDefaultAsync(c => c.Estado == "Abierta");

                if (caja == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("No hay caja abierta"));

                return Ok(ApiResponse<Caja>.SuccessResponse(caja, "Caja activa encontrada"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar caja activa");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Error al consultar caja",
                    new List<string> { ex.Message }));
            }
        }

        // GET: api/cajas/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Caja>>> GetCaja(int id)
        {
            try
            {
                var caja = await _context.Cajas.FindAsync(id);
                if (caja == null)
                    return NotFound(ApiResponse<object>.ErrorResponse($"Caja con ID {id} no encontrada"));

                return Ok(ApiResponse<Caja>.SuccessResponse(caja, "Caja encontrada"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al consultar caja ID: {id}");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Error al consultar caja",
                    new List<string> { ex.Message }));
            }
        }

        // POST: api/cajas
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Caja>>> PostCaja(Caja caja)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Datos inválidos",
                        ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));

                // Verificar si hay caja abierta
                var cajaAbierta = await _context.Cajas.AnyAsync(c => c.Estado == "Abierta");
                if (cajaAbierta)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Ya existe una caja abierta"));

                caja.SaldoEfectivo = caja.SaldoInicial;
                caja.Estado = "Abierta";
                caja.FechaApertura = DateTime.Now;

                _context.Cajas.Add(caja);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Caja abierta: {caja.IdCaja}");
                return CreatedAtAction(nameof(GetCaja), new { id = caja.IdCaja },
                    ApiResponse<Caja>.SuccessResponse(caja, "Caja abierta exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al abrir caja");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Error al abrir caja",
                    new List<string> { ex.Message }));
            }
        }

        // PUT: api/cajas/{id}/cerrar
        [HttpPut("{id}/cerrar")]
        public async Task<ActionResult<ApiResponse<Caja>>> CerrarCaja(int id, [FromBody] decimal saldoFinal)
        {
            try
            {
                var caja = await _context.Cajas.FindAsync(id);
                if (caja == null)
                    return NotFound(ApiResponse<object>.ErrorResponse($"Caja con ID {id} no encontrada"));

                if (caja.Estado != "Abierta")
                    return BadRequest(ApiResponse<object>.ErrorResponse("La caja no está abierta"));

                caja.Estado = "Cerrada";
                caja.FechaCierre = DateTime.Now;
                caja.SaldoFinal = saldoFinal;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Caja cerrada: {id}");
                return Ok(ApiResponse<Caja>.SuccessResponse(caja, "Caja cerrada exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al cerrar caja ID: {id}");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Error al cerrar caja",
                    new List<string> { ex.Message }));
            }
        }
    }
}
