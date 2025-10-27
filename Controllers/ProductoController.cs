using ECommerceAPI.Data;
using ECommerceAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous] // ¡LA CLAVE! Esto permite el acceso sin token.
    public class ProductoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Solo necesitamos el DbContext para leer datos
        public ProductoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ProductoResponseDto>), 200)]
        public async Task<IActionResult> GetAllProductos()
        {
            // Busca todos los productos, e incluye la info de la Empresa
            var productos = await _context.Productos
                .Include(p => p.Empresa) // ¡Importante para saber el nombre de la empresa!
                .AsNoTracking() // Mejora el rendimiento, es solo lectura
                .Select(p => new ProductoResponseDto // Mapea al DTO de respuesta
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    Stock = p.Stock,
                    ImagenUrl = p.ImagenUrl,
                    EmpresaId = p.EmpresaId,
                    NombreEmpresa = p.Empresa.Nombre // Mapeamos el nombre
                })
                .ToListAsync();

            return Ok(productos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductoResponseDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProductoPorId(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Empresa)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
            {
                return NotFound(new { Message = "Producto no encontrado." });
            }

            // Mapeamos al DTO de respuesta
            var productoDto = new ProductoResponseDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Stock = producto.Stock,
                ImagenUrl = producto.ImagenUrl,
                EmpresaId = producto.EmpresaId,
                NombreEmpresa = producto.Empresa.Nombre
            };

            return Ok(productoDto);
        }

        [HttpGet("empresa/{empresaId}")]
        [ProducesResponseType(typeof(List<ProductoResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProductosPorEmpresa(int empresaId)
        {
            // Valida si la empresa existe
            var empresaExiste = await _context.Empresas.AnyAsync(e => e.Id == empresaId);
            if (!empresaExiste)
            {
                return NotFound(new { Message = "La empresa especificada no existe." });
            }

            // Busca todos los productos de ESA empresa
            var productos = await _context.Productos
                .Where(p => p.EmpresaId == empresaId)
                .Include(p => p.Empresa)
                .AsNoTracking()
                .Select(p => new ProductoResponseDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    Stock = p.Stock,
                    ImagenUrl = p.ImagenUrl,
                    EmpresaId = p.EmpresaId,
                    NombreEmpresa = p.Empresa.Nombre
                })
                .ToListAsync();

            // Devuelve la lista (puede estar vacía si la empresa no tiene productos)
            return Ok(productos);
        }
    }
}