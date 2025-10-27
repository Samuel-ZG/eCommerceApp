using System.Collections.Generic;

namespace ECommerceAPI.Models.DTOs
{
    // Esto es lo que enviaremos al usuario (Flutter) cuando haga login
    public class AuthResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string? Token { get; set; }
        public string? Email { get; set; }
        public List<string>? Roles { get; set; }
    }
}