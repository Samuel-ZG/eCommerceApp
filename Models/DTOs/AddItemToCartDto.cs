using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models.DTOs
{
    public class AddItemToCartDto
    {
        [Required]
        public int ProductoId { get; set; }
        
        [Required]
        [Range(1, 100)]
        public int Cantidad { get; set; }
    }
}