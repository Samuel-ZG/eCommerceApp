using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models.DTOs
{
    public class CrearEmpresaDto
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; }

        [Required(ErrorMessage = "El nombre de la empresa es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El rubro es obligatorio")]
        public string Rubro { get; set; }
    }
}