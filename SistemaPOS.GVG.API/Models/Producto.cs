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
        [Range(0, 999999.99, ErrorMessage = "El precio de costo debe estar entre 0 y 999999.99")]
        public decimal PrecioCosto { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999.99, ErrorMessage = "El precio de venta debe estar entre 0 y 999999.99")]
        public decimal PrecioVenta { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 9999999.99, ErrorMessage = "El stock debe ser un número válido")]
        public decimal Stock { get; set; } = 0;
    }
}