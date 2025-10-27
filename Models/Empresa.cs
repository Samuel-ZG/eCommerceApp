using ECommerceAPI.Data; // Importante para ApplicationUser
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.Models
{
    public class Empresa
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(200)]
        public string Rubro { get; set; }

        public DateTime FechaRegistro { get; set; }

        // --- ¡ESTA ES LA PARTE QUE FALTABA! ---
        // Clave foránea para relacionar con ApplicationUser
        [Required]
        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        // ------------------------------------

        // Relación uno-a-muchos: Una empresa tiene muchos productos
        public virtual ICollection<Producto> Productos { get; set; }

        public Empresa()
        {
            Productos = new HashSet<Producto>();
        }
    }
}