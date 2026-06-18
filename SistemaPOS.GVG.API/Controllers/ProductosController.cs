using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPOS.API.Data;
using SistemaPOS.API.Models;

namespace SistemaPOS.API.Controllers
{
    // Esta ruta define que el endpoint será: http://localhost:puerto/api/productos
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Inyección de dependencias del DbContext
        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Productos
        // Retorna el catálogo completo de pinturas y consumibles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos.ToListAsync();
        }

        // POST: api/Productos
        // Permite registrar un nuevo producto en el sistema
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            // Retorna un código 201 (Created) y el objeto recién insertado
            return CreatedAtAction(nameof(GetProductos), new { id = producto.IdProducto }, producto);
        }
    }
}