using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models.DTOs
{
    public class CreateUpdateProductoDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, 1000000, ErrorMessage = "El precio debe ser positivo")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio")]
        [Range(0, 100000, ErrorMessage = "El stock debe ser 0 o más")]
        public int Stock { get; set; }

        public string? ImagenUrl { get; set; }
    }
}