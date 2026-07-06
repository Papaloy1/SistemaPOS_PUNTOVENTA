using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPOS.API.Models
{
    public class Caja
    {
        [Key]
        public int IdCaja { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreCaja { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Ubicacion { get; set; }

        public DateTime FechaApertura { get; set; } = DateTime.Now;

        public DateTime? FechaCierre { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SaldoInicial { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal SaldoEfectivo { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalVentas { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalEgresos { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal SaldoFinal { get; set; } = 0;

        [StringLength(20)]
        public string Estado { get; set; } = "Abierta";

        public int UsuarioId { get; set; } = 1;

        public string? Notas { get; set; }
    }
}
