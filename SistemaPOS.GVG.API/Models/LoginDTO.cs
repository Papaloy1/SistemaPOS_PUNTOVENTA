using System.ComponentModel.DataAnnotations;

namespace SistemaPOS.GVG.API.Models
{
    public class LoginDTO
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}