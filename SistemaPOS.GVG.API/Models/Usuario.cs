using System.ComponentModel.DataAnnotations;

namespace SistemaPOS.GVG.API.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Rol { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;
    }
}