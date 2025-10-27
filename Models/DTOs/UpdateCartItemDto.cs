using System.ComponentModel.DataAnnotations;
    
namespace ECommerceAPI.Models.DTOs
{
    public class UpdateCartItemDto
    {
        [Required]
        [Range(0, 100)] // Permitir 0 para eliminar
        public int NuevaCantidad { get; set; }
    }
}