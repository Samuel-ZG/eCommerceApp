using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using ECommerceAPI.Models; // Importante: para conectar con nuestros modelos

namespace ECommerceAPI.Data
{
    // Hereda de IdentityUser para tener Email, PasswordHash, etc.
    public class ApplicationUser : IdentityUser
    {
        // --- Relaciones de E-Commerce ---

        // Un usuario (con rol Empresa) puede tener muchas empresas
        public virtual ICollection<Empresa> Empresas { get; set; }

        // Un usuario (con rol Usuario) puede tener muchos pedidos
        public virtual ICollection<Pedido> Pedidos { get; set; }

        public ApplicationUser()
        {
            Empresas = new HashSet<Empresa>();
            Pedidos = new HashSet<Pedido>();
        }
    }
}