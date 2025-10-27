namespace ECommerceAPI.Models.DTOs
{
    // DTO para un item individual
    public class DetallePedidoResponseDto 
    {
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public double PrecioUnitario { get; set; } // Flutter prefiere double
    }

    // DTO para el pedido completo
    public class PedidoResponseDto
    {
        public int Id { get; set; }
        public string FechaPedido { get; set; } // Usaremos la 'Fecha' de tu modelo
        public double Total { get; set; }
        public List<DetallePedidoResponseDto> Items { get; set; } // Un nombre genérico

        public PedidoResponseDto()
        {
            Items = new List<DetallePedidoResponseDto>();
        }
    }
}