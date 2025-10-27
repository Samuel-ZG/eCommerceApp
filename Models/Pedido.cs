using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ECommerceAPI.Data; // Para conectar con ApplicationUser

namespace ECommerceAPI.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }

        public string UsuarioId { get; set; }
        public virtual ApplicationUser Usuario { get; set; }

        public virtual ICollection<DetallePedido> Detalles { get; set; }

        public Pedido()
        {
            Detalles = new HashSet<DetallePedido>();
            Fecha = DateTime.UtcNow;
        }
    }
}