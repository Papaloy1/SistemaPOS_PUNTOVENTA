namespace SistemaPOS.Desktop.Models
{
    public class ClienteDTO
    {
        public int IdCliente { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Direccion { get; set; }
        public string Ciudad { get; set; }
        public string Estado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public decimal SaldoCredito { get; set; }
        public bool Activo { get; set; }
    }
}
