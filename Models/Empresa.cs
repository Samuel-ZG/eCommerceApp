using System.Collections.Generic;
using ECommerceAPI.Data; // Para conectar con ApplicationUser

namespace ECommerceAPI.Models
{
    public class Empresa
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string? LogoUrl { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Producto> Productos { get; set; }

        public Empresa()
        {
            Productos = new HashSet<Producto>();
        }
    }
}