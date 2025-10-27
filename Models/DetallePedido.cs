using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.Models
{
    public class DetallePedido
    {
        [Key]
        public int Id { get; set; }

        // --- Relación con Pedido ---
        [Required]
        public int PedidoId { get; set; }
        [ForeignKey("PedidoId")]
        public virtual Pedido Pedido { get; set; }

        // --- Relación con Producto (CORREGIDA) ---
        // Hacemos que sea "nulleable" (con '?')
        public int? ProductoId { get; set; } // <-- CAMBIO 1
        [ForeignKey("ProductoId")]
        public virtual Producto? Producto { get; set; } // <-- CAMBIO 2

        [Required]
        public int Cantidad { get; set; }

        // --- "Foto" del producto (CORREGIDA) ---
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; } 

        [Required]
        public string NombreProducto { get; set; } // <-- CAMBIO 3 (¡AÑADIDO!)
    }
}