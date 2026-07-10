using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPOS.API.Models
{
    public class Venta
    {
        [Key]
        public int IdVenta { get; set; }

        [ForeignKey("Cliente")]
        public int? IdCliente { get; set; }
        public virtual Cliente? Cliente { get; set; }

        public DateTime FechaVenta { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Impuesto { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Pagado { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Cambio { get; set; }

        [StringLength(20)]
        public string TipoPago { get; set; } = "Efectivo";

        [StringLength(500)]
        public string? Observaciones { get; set; }

        public int UsuarioId { get; set; } = 1;

        public bool Cancelada { get; set; } = false;

        // Navegación
        public virtual ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}
