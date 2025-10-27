using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models.DTOs
{
    public class RegisterEmpresaRequestDto
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        public string Username { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; }

        [Required(ErrorMessage = "El nombre de la empresa es obligatorio")]
        public string NombreEmpresa { get; set; }

        public string? DescripcionEmpresa { get; set; }
    }
}