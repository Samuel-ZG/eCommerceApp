using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.Models
{
    public class DetallePedido
    {
        public int Id { get; set; }
        public int Cantidad { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PrecioUnitario { get; set; }

        public int PedidoId { get; set; }
        public virtual Pedido Pedido { get; set; }

        public int ProductoId { get; set; }
        public virtual Producto Producto { get; set; }
    }
}