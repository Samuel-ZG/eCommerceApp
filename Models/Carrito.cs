using ECommerceAPI.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.Models
{
    public class Carrito
    {
        [Key]
        public int Id { get; set; }

        // Clave foránea al usuario
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public List<CarritoItem> Items { get; set; } = new List<CarritoItem>();
    }
}