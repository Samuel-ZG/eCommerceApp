using ECommerceAPI.Data; // Para ApplicationUser
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.Models
{
    public class Carrito
    {
        [Key]
        public int Id { get; set; }

        // --- Clave foránea para el Usuario ---
        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        // --- ¡ESTA ES LA LÍNEA MÁS IMPORTANTE! ---
        // Relación 1-a-Muchos: Un carrito tiene muchos items
        public virtual ICollection<CarritoItem> Items { get; set; }

        public Carrito()
        {
            // Inicializa la lista para que nunca sea nula
            Items = new HashSet<CarritoItem>();
        }
    }
}