using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPOS.API.Models
{
    public class Cliente
    {
        [Key]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Telefono { get; set; }

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "El correo debe ser válido")]
        public string? Correo { get; set; }

        [StringLength(200)]
        public string? Direccion { get; set; }

        [StringLength(50)]
        public string? Ciudad { get; set; }

        [StringLength(50)]
        public string? Estado { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal SaldoCredito { get; set; } = 0;

        public bool Activo { get; set; } = true;

        // Navegación
        public virtual ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    }
}
