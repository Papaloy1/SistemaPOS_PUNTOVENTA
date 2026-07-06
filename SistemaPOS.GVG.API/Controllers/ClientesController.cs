using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPOS.API.Data;
using SistemaPOS.API.Models;

namespace SistemaPOS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(AppDbContext context, ILogger<ClientesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/clientes
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Cliente>>>> GetClientes()
        {
            try
            {
                _logger.LogInformation("Consultando todos los clientes");
                var clientes = await _context.Clientes.Where(c => c.Activo).ToListAsync();
                return Ok(ApiResponse<IEnumerable<Cliente>>.SuccessResponse(clientes,
                    $"Se encontraron {clientes.Count} clientes"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar clientes");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Error al consultar clientes",
                    new List<string> { ex.Message }));
            }
        }

        // GET: api/clientes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Cliente>>> GetCliente(int id)
        {
            try
            {
                var cliente = await _context.Clientes.FindAsync(id);
                if (cliente == null)
                    return NotFound(ApiResponse<object>.ErrorResponse($"Cliente con ID {id} no encontrado"));

                return Ok(ApiResponse<Cliente>.SuccessResponse(cliente, "Cliente encontrado"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al consultar cliente ID: {id}");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Error al consultar cliente",
                    new List<string> { ex.Message }));
            }
        }

        // POST: api/clientes
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Cliente>>> PostCliente(Cliente cliente)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Datos inválidos",
                        ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));

                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Cliente creado: {cliente.IdCliente}");
                return CreatedAtAction(nameof(GetCliente), new { id = cliente.IdCliente },
                    ApiResponse<Cliente>.SuccessResponse(cliente, "Cliente creado exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear cliente");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Error al crear cliente",
                    new List<string> { ex.Message }));
            }
        }

        // PUT: api/clientes/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Cliente>>> PutCliente(int id, Cliente cliente)
        {
            if (id != cliente.IdCliente)
                return BadRequest(ApiResponse<object>.ErrorResponse("ID no coincide"));

            try
            {
                _context.Entry(cliente).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Cliente actualizado: {id}");
                return Ok(ApiResponse<Cliente>.SuccessResponse(cliente, "Cliente actualizado exitosamente"));
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"Cliente con ID {id} no encontrado"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar cliente ID: {id}");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Error al actualizar cliente",
                    new List<string> { ex.Message }));
            }
        }

        // DELETE: api/clientes/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteCliente(int id)
        {
            try
            {
                var cliente = await _context.Clientes.FindAsync(id);
                if (cliente == null)
                    return NotFound(ApiResponse<object>.ErrorResponse($"Cliente con ID {id} no encontrado"));

                cliente.Activo = false;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Cliente desactivado: {id}");
                return Ok(ApiResponse<object>.SuccessResponse(null, "Cliente desactivado exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar cliente ID: {id}");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Error al eliminar cliente",
                    new List<string> { ex.Message }));
            }
        }

        // GET: api/clientes/buscar/{nombre}
        [HttpGet("buscar/{nombre}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Cliente>>>> BuscarClientes(string nombre)
        {
            try
            {
                var clientes = await _context.Clientes
                    .Where(c => c.Activo && c.Nombre.ToLower().Contains(nombre.ToLower()))
                    .ToListAsync();

                return Ok(ApiResponse<IEnumerable<Cliente>>.SuccessResponse(clientes,
                    $"Se encontraron {clientes.Count} clientes"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al buscar clientes por nombre: {nombre}");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Error al buscar clientes",
                    new List<string> { ex.Message }));
            }
        }
    }
}
