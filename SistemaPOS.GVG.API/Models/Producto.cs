using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPOS.API.Models
{
    public class Producto
    {
        [Key]
        public int IdProducto { get; set; }

        public string CodigoBarras { get; set; }

        [Required]
        public string Descripcion { get; set; }

        public string Categoria { get; set; }
        public string Acabado { get; set; }
        public string Tamanio { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioCosto { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioVenta { get; set; }
    }
}