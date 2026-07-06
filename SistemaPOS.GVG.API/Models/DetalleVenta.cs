using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPOS.API.Models
{
    public class DetalleVenta
    {
        [Key]
        public int IdDetalle { get; set; }

        [ForeignKey("Venta")]
        public int IdVenta { get; set; }
        public virtual Venta? Venta { get; set; }

        [ForeignKey("Producto")]
        public int IdProducto { get; set; }
        public virtual Producto? Producto { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Cantidad { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Descuento { get; set; } = 0;
    }
}
