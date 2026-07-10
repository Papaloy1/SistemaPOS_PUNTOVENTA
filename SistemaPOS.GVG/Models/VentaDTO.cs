namespace SistemaPOS.Desktop.Models
{
    public class VentaDTO
    {
        public int IdVenta { get; set; }
        public int? IdCliente { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }
        public decimal Pagado { get; set; }
        public decimal Cambio { get; set; }
        public string TipoPago { get; set; }
        public string Observaciones { get; set; }
        public int UsuarioId { get; set; }
        public bool Cancelada { get; set; }
    }
}
