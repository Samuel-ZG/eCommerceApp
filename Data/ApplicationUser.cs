using Microsoft.AspNetCore.Identity;
using ECommerceAPI.Models; // Importante: para conectar con nuestros modelos

namespace ECommerceAPI.Data
{
    // Hereda de IdentityUser para tener Email, PasswordHash, etc.
    public class ApplicationUser : IdentityUser
    {
        // Propiedades que teníamos antes (si las quieres mantener)
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }

        // --- RELACIÓN CON CARRITO (Añadida en Paso 6) ---
        // Un usuario tiene un carrito
        public virtual Carrito? Carrito { get; set; }

        // NOTA: Las relaciones de Empresa y Pedido las quitamos
        // porque las estamos manejando de otra forma (UserId en Empresa)
        // o aún no las hemos implementado (Pedido).
    }
}