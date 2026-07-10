namespace SistemaPOS.Desktop.Models
{
    public class CajaDTO
    {
        public int IdCaja { get; set; }
        public string NombreCaja { get; set; }
        public string Ubicacion { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public decimal SaldoInicial { get; set; }
        public decimal SaldoEfectivo { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal TotalEgresos { get; set; }
        public decimal SaldoFinal { get; set; }
        public string Estado { get; set; }
        public int UsuarioId { get; set; }
        public string Notas { get; set; }
    }
}
