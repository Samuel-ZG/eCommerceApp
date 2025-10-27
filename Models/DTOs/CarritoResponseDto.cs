namespace ECommerceAPI.Models.DTOs
{
    // DTO para un item individual
    // ¡Este SÍ incluye el Id del item!
    public class CarritoItemResponseDto
    {
        public int Id { get; set; } // <-- El ID del CarritoItem (para borrar)
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public double Precio { get; set; }
        public int Cantidad { get; set; }
        public string? ImageUrl { get; set; }
    }

    // DTO para el carrito completo
    public class CarritoResponseDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public List<CarritoItemResponseDto> Items { get; set; }
        public double Total { get; set; }
        
        public CarritoResponseDto()
        {
            Items = new List<CarritoItemResponseDto>();
        }
    }
}