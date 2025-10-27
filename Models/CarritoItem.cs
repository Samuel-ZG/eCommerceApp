using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.Models
{
    public class CarritoItem
    {
        [Key]
        public int Id { get; set; }

        public int Cantidad { get; set; }

        // Clave foránea al carrito
        public int CarritoId { get; set; }
        [ForeignKey("CarritoId")]
        public Carrito Carrito { get; set; }

        // Clave foránea al producto
        public int ProductoId { get; set; }
        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; }
    }
}