using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPOS.API.Models
{
    public class Producto
    {
        [Key]
        public int IdProducto { get; set; }

        [Required]
        public required string CodigoBarras { get; set; }

        [Required]
        public required string Descripcion { get; set; }

        [Required]
        public required string Categoria { get; set; }

        [Required]
        public required string Acabado { get; set; }

        [Required]
        public required string Tamanio { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioCosto { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioVenta { get; set; }
    }
}