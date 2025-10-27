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
        [Authorize(Roles = "Usuario")] // ¡LA CLAVE! Solo usuarios normales
        public class CarritoController : ControllerBase
        {
            private readonly ApplicationDbContext _context;
            private readonly UserManager<ApplicationUser> _userManager;

            public CarritoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
            {
                _context = context;
                _userManager = userManager;
            }

            // --- MÉTODO PRIVADO DE AYUDA ---
            private async Task<Carrito?> GetMyCartAsync()
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return null; // No debería pasar si está autorizado
                }

                // Busca el carrito, incluyendo los items, sus productos y la empresa
                var carrito = await _context.Carritos
                    .Include(c => c.Items)
                        .ThenInclude(item => item.Producto)
                            .ThenInclude(p => p.Empresa)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                // Si el usuario no tiene carrito (primera vez), se lo creamos
                if (carrito == null)
                {
                    carrito = new Carrito { UserId = userId };
                    _context.Carritos.Add(carrito);
                    await _context.SaveChangesAsync();
                }

                return carrito;
            }

            // --- MÉTODO PRIVADO DE MAPEO ---
            private CartResponseDto MapCartToDto(Carrito carrito)
            {
                var itemsDto = carrito.Items.Select(item => new CartItemResponseDto
                {
                    ProductoId = item.ProductoId,
                    NombreProducto = item.Producto.Nombre,
                    PrecioProducto = item.Producto.Precio,
                    ImagenUrl = item.Producto.ImagenUrl,
                    NombreEmpresa = item.Producto.Empresa.Nombre,
                    Cantidad = item.Cantidad,
                    SubTotal = item.Producto.Precio * item.Cantidad
                }).ToList();

                return new CartResponseDto
                {
                    CarritoId = carrito.Id,
                    UserId = carrito.UserId,
                    Items = itemsDto,
                    Total = itemsDto.Sum(item => item.SubTotal)
                };
            }

            // --- ENDPOINTS ---

            [HttpGet]
            public async Task<IActionResult> GetMyCart()
            {
                var carrito = await GetMyCartAsync();
                if (carrito == null)
                {
                    return Unauthorized();
                }

                return Ok(MapCartToDto(carrito));
            }

            [HttpPost("add")]
            public async Task<IActionResult> AddItemToCart([FromBody] AddItemToCartDto itemDto)
            {
                var carrito = await GetMyCartAsync();
                if (carrito == null) return Unauthorized();

                // 1. Validar el producto
                var producto = await _context.Productos.FindAsync(itemDto.ProductoId);
                if (producto == null)
                {
                    return NotFound(new { Message = "Producto no encontrado." });
                }
                if (producto.Stock < itemDto.Cantidad)
                {
                    return BadRequest(new { Message = $"Stock insuficiente. Solo quedan {producto.Stock}." });
                }

                // 2. Revisar si el item ya está en el carrito
                var itemEnCarrito = carrito.Items.FirstOrDefault(i => i.ProductoId == itemDto.ProductoId);

                if (itemEnCarrito != null)
                {
                    // Si ya existe, actualiza la cantidad
                    itemEnCarrito.Cantidad += itemDto.Cantidad;
                }
                else
                {
                    // Si no existe, lo crea
                    var nuevoItem = new CarritoItem
                    {
                        CarritoId = carrito.Id,
                        ProductoId = itemDto.ProductoId,
                        Cantidad = itemDto.Cantidad
                    };
                    _context.CarritoItems.Add(nuevoItem);
                }

                await _context.SaveChangesAsync();

                // Recargar el carrito con los datos actualizados para devolverlo
                var carritoActualizado = await GetMyCartAsync();
                return Ok(MapCartToDto(carritoActualizado!));
            }

            [HttpPut("update/{productoId}")]
            public async Task<IActionResult> UpdateCartItem(int productoId, [FromBody] UpdateCartItemDto itemDto)
            {
                var carrito = await GetMyCartAsync();
                if (carrito == null) return Unauthorized();

                var itemEnCarrito = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
                if (itemEnCarrito == null)
                {
                    return NotFound(new { Message = "Producto no encontrado en el carrito." });
                }

                if (itemDto.NuevaCantidad == 0)
                {
                    // Si la cantidad es 0, lo elimina
                    _context.CarritoItems.Remove(itemEnCarrito);
                }
                else
                {
                    // Valida el stock
                    var producto = await _context.Productos.FindAsync(productoId);
                    if (producto.Stock < itemDto.NuevaCantidad)
                    {
                        return BadRequest(new { Message = $"Stock insuficiente. Solo quedan {producto.Stock}." });
                    }
                    itemEnCarrito.Cantidad = itemDto.NuevaCantidad;
                }
                
                await _context.SaveChangesAsync();
                
                var carritoActualizado = await GetMyCartAsync();
                return Ok(MapCartToDto(carritoActualizado!));
            }

            [HttpDelete("remove/{productoId}")]
            public async Task<IActionResult> RemoveItemFromCart(int productoId)
            {
                var carrito = await GetMyCartAsync();
                if (carrito == null) return Unauthorized();

                var itemEnCarrito = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
                if (itemEnCarrito == null)
                {
                    return NotFound(new { Message = "Producto no encontrado en el carrito." });
                }

                _context.CarritoItems.Remove(itemEnCarrito);
                await _context.SaveChangesAsync();
                
                var carritoActualizado = await GetMyCartAsync();
                return Ok(MapCartToDto(carritoActualizado!));
            }
        }
    }