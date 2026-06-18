using Microsoft.EntityFrameworkCore;
using SistemaPOS.API.Models;

namespace SistemaPOS.API.Data
{
    public class AppDbContext : DbContext
    {
        // El constructor recibe las opciones de conexión y las pasa a la clase base
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Declaración de las tablas (DbSets)
        public DbSet<Producto> Productos { get; set; }
        // public DbSet<Sucursal> Sucursales { get; set; }
        // public DbSet<InventarioSucursal> InventarioSucursales { get; set; }
        // public DbSet<Traspaso> Traspasos { get; set; }
    }
}