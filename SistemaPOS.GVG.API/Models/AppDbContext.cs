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
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }
        public DbSet<Caja> Cajas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de relaciones
            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Cliente)
                .WithMany(c => c.Ventas)
                .HasForeignKey(v => v.IdCliente)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<DetalleVenta>()
                .HasOne(dv => dv.Venta)
                .WithMany(v => v.Detalles)
                .HasForeignKey(dv => dv.IdVenta)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetalleVenta>()
                .HasOne(dv => dv.Producto)
                .WithMany()
                .HasForeignKey(dv => dv.IdProducto)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}