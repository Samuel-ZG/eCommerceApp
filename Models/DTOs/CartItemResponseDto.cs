namespace ECommerceAPI.Models.DTOs
{
    // DTO para un item individual
    public class CartItemResponseDto
    {
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; }
        public decimal PrecioProducto { get; set; }
        public string? ImagenUrl { get; set; }
        public string NombreEmpresa { get; set; }
        public int Cantidad { get; set; }
        public decimal SubTotal { get; set; }
    }
    
    // DTO para el carrito completo
    public class CartResponseDto
    {
        public int CarritoId { get; set; }
        public string UserId { get; set; }
        public List<CartItemResponseDto> Items { get; set; }
        public decimal Total { get; set; }
    }
}