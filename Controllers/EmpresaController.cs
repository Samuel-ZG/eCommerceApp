using ECommerceAPI.Data;
using ECommerceAPI.Models;
using ECommerceAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims; // ¡Importante para leer el Token!

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Empresa")] // ¡Solo usuarios con rol "Empresa" pueden usar esto!
    public class EmpresaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmpresaController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // --- Método Privado de Ayuda ---
        // Obtiene el ID de la Empresa que está asociada al Usuario (del token)
        private async Task<int?> GetMyEmpresaIdAsync()
        {
            // 1. Obtener el ID de usuario (GUID) desde el token JWT
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            // 2. Buscar en la tabla Empresas cuál empresa está ligada a ese UserId
            // --- ¡AQUÍ ESTÁ LA CORRECCIÓN! ---
            var empresa = await _context.Empresas
                                        .AsNoTracking() // Más rápido, solo lectura
                                        .FirstOrDefaultAsync(e => e.ApplicationUserId == userId); // <-- CAMBIO DE UserId a ApplicationUserId

            if (empresa == null)
            {
                return null; // Este usuario no está ligado a ninguna empresa
            }

            // 3. Devolver el ID (int) de esa empresa
            return empresa.Id;
        }


        // --- ENDPOINTS CRUD DE PRODUCTOS ---

        [HttpGet("productos")]
        public async Task<IActionResult> GetMisProductos()
        {
            var empresaId = await GetMyEmpresaIdAsync();
            if (empresaId == null)
            {
                return Unauthorized(new { Message = "Este usuario no está asociado a ninguna empresa." });
            }

            // Busca solo los productos DONDE EmpresaId == mi_empresa_id
            var productos = await _context.Productos
                .Where(p => p.EmpresaId == empresaId)
                .Select(p => new ProductoResponseDto // Mapea a DTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    Stock = p.Stock,
                    ImagenUrl = p.ImagenUrl,
                    EmpresaId = p.EmpresaId,
                    NombreEmpresa = p.Empresa.Nombre // Incluye el nombre de la empresa
                })
                .ToListAsync();

            return Ok(productos);
        }

        [HttpPost("productos")] // CAMBIADO DE "productos" a "crear-producto" para ser más claro
        public async Task<IActionResult> CreateProducto([FromBody] CreateUpdateProductoDto productoDto)
        {
            var empresaId = await GetMyEmpresaIdAsync();
            if (empresaId == null)
            {
                return Unauthorized(new { Message = "Este usuario no está asociado a ninguna empresa." });
            }

            var nuevoProducto = new Producto
            {
                Nombre = productoDto.Nombre,
                Descripcion = productoDto.Descripcion ?? "",
                Precio = productoDto.Precio,
                Stock = productoDto.Stock,
                ImagenUrl = productoDto.ImagenUrl,
                EmpresaId = empresaId.Value // ¡LA CLAVE DE SEGURIDAD! Asigna el producto a esta empresa
            };

            _context.Productos.Add(nuevoProducto);
            await _context.SaveChangesAsync();

            // Devolver un DTO del producto recién creado (aunque no es 100% necesario)
            return CreatedAtAction(nameof(GetProductoPorId), new { id = nuevoProducto.Id }, nuevoProducto);
        }

        [HttpGet("productos/{id}")]
        public async Task<IActionResult> GetProductoPorId(int id)
        {
            var empresaId = await GetMyEmpresaIdAsync();
            if (empresaId == null)
            {
                return Unauthorized(new { Message = "Usuario no asociado a empresa." });
            }

            var producto = await _context.Productos
                                        .Include(p => p.Empresa) // Carga la info de la Empresa
                                        .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
            {
                return NotFound(new { Message = "Producto no encontrado." });
            }

            // ¡SEGURIDAD! Verifica si el producto pertenece a la empresa que hace la petición
            if (producto.EmpresaId != empresaId)
            {
                return Forbid(); // 403 Forbidden - Encontrado, pero no tienes permiso
            }

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


        [HttpPut("productos/{id}")]
        public async Task<IActionResult> UpdateProducto(int id, [FromBody] CreateUpdateProductoDto productoDto)
        {
            var empresaId = await GetMyEmpresaIdAsync();
            if (empresaId == null)
            {
                return Unauthorized(new { Message = "Usuario no asociado a empresa." });
            }

            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
            {
                return NotFound(new { Message = "Producto no encontrado." });
            }

            // ¡SEGURIDAD! Solo puede editar si el producto es suyo
            if (producto.EmpresaId != empresaId)
            {
                return Forbid(); // 403 Forbidden
            }

            // Actualizar los campos
            producto.Nombre = productoDto.Nombre;
            producto.Descripcion = productoDto.Descripcion ?? producto.Descripcion;
            producto.Precio = productoDto.Precio;
            producto.Stock = productoDto.Stock;
            producto.ImagenUrl = productoDto.ImagenUrl ?? producto.ImagenUrl;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { Message = "Error al actualizar el producto." });
            }

            return Ok(new { Message = "Producto actualizado exitosamente." });
        }

        [HttpDelete("productos/{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var empresaId = await GetMyEmpresaIdAsync();
            if (empresaId == null)
            {
                return Unauthorized(new { Message = "Usuario no asociado a empresa." });
            }

            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
            {
                return NotFound(new { Message = "Producto no encontrado." });
            }

            // ¡SEGURIDAD! Solo puede borrar si el producto es suyo
            if (producto.EmpresaId != empresaId)
            {
                return Forbid(); // 403 Forbidden
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Producto eliminado exitosamente." });
        }
    }
}

