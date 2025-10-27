namespace ECommerceAPI.Models.DTOs
{
    // Usaremos este DTO para devolver los productos al cliente
    public class ProductoResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string? ImagenUrl { get; set; }
        public int EmpresaId { get; set; }
        public string NombreEmpresa { get; set; } // Para que el frontend sepa de quién es
    }
}