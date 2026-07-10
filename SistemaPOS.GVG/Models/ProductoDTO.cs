namespace SistemaPOS.Desktop.Models
{
    public class ProductoDTO
    {
        public int IdProducto { get; set; }
        public string CodigoBarras { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public string Acabado { get; set; }
        public string Tamanio { get; set; }
        public decimal PrecioCosto { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Stock { get; set; }
    }
}