using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPOS.API.Models
{
    public class Producto
    {
        [Key]
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "El código de barras es obligatorio")]
        [StringLength(50, ErrorMessage = "El código de barras no puede exceder 50 caracteres")]
        public string CodigoBarras { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "La descripción debe tener entre 3 y 200 caracteres")]
        public string Descripcion { get; set; }

        [StringLength(50, ErrorMessage = "La categoría no puede exceder 50 caracteres")]
        public string Categoria { get; set; }

        [StringLength(50, ErrorMessage = "El acabado no puede exceder 50 caracteres")]
        public string Acabado { get; set; }

        [StringLength(50, ErrorMessage = "El tamaño no puede exceder 50 caracteres")]
        public string Tamanio { get; set; }

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