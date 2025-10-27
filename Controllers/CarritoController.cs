using ECommerceAPI.Data;
using ECommerceAPI.Models;
using ECommerceAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CarritoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CarritoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // --- ¡MÉTODO CRÍTICO! ---
        // Carga el carrito CON sus items y productos
        private async Task<Carrito> GetOrCreateCarritoAsync(string userId)
        {
            var carrito = await _context.Carritos
                // --- ¡ESTA LÍNEA DEPENDE DEL MODELO QUE ACABAMOS DE ARREGLAR! ---
                .Include(c => c.Items)                 
                    .ThenInclude(item => item.Producto) 
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (carrito == null)
            {
                carrito = new Carrito { UserId = userId };
                _context.Carritos.Add(carrito);
                await _context.SaveChangesAsync();
            }

            return carrito;
        }

        // --- ENDPOINT GET (CORREGIDO) ---
        [HttpGet]
        public async Task<IActionResult> GetMyCarrito()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var carrito = await GetOrCreateCarritoAsync(userId); 

            var dto = new CarritoResponseDto
            {
                Id = carrito.Id,
                UserId = carrito.UserId,
                Items = carrito.Items.Select(item => new CarritoItemResponseDto
                {
                    Id = item.Id,
                    ProductoId = item.ProductoId,
                    ProductoNombre = item.Producto?.Nombre ?? "Producto no disponible",
                    Precio = (double)(item.Producto?.Precio ?? 0m), // Corrección decimal
                    Cantidad = item.Cantidad,
                    ImageUrl = item.Producto?.ImagenUrl ?? ""
                }).ToList()
            };

            dto.Total = dto.Items.Sum(item => item.Precio * item.Cantidad);
            return Ok(dto);
        }

        // --- ENDPOINT POST (Añadir) ---
        [HttpPost("add")]
        public async Task<IActionResult> AddItemToCarrito([FromBody] AddItemToCartDto itemDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var carrito = await _context.Carritos.FirstOrDefaultAsync(c => c.UserId == userId);
            if (carrito == null)
            {
                carrito = new Carrito { UserId = userId };
                _context.Carritos.Add(carrito);
                await _context.SaveChangesAsync();
            }

            var producto = await _context.Productos.FindAsync(itemDto.ProductoId);
            if (producto == null) return NotFound(new { message = "Producto no encontrado." });
            if (producto.Stock < itemDto.Cantidad) return BadRequest(new { message = "Stock insuficiente." });

            var itemEnCarrito = await _context.CarritoItems
                .FirstOrDefaultAsync(ci => ci.CarritoId == carrito.Id && ci.ProductoId == itemDto.ProductoId);

            if (itemEnCarrito != null)
            {
                itemEnCarrito.Cantidad += itemDto.Cantidad;
            }
            else
            {
                var nuevoItem = new CarritoItem
                {
                    CarritoId = carrito.Id,
                    ProductoId = itemDto.ProductoId,
                    Cantidad = itemDto.Cantidad,
                };
                _context.CarritoItems.Add(nuevoItem);
            }

            producto.Stock -= itemDto.Cantidad;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Producto añadido al carrito." });
        }


        // --- ENDPOINT DELETE (Quitar) ---
        [HttpDelete("remove/{itemId}")]
        public async Task<IActionResult> RemoveItemFromCarrito(int itemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var carrito = await _context.Carritos.FirstOrDefaultAsync(c => c.UserId == userId);
            if (carrito == null) return NotFound(new { message = "Carrito no encontrado." });

            var itemEnCarrito = await _context.CarritoItems
                .Include(ci => ci.Producto) 
                .FirstOrDefaultAsync(ci => ci.CarritoId == carrito.Id && ci.Id == itemId);

            if (itemEnCarrito == null) return NotFound(new { message = "Item no encontrado en el carrito." });

            if (itemEnCarrito.Producto != null)
            {
                itemEnCarrito.Producto.Stock += itemEnCarrito.Cantidad;
            }

            _context.CarritoItems.Remove(itemEnCarrito);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Item eliminado del carrito." });
        }
    }
}