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
    public class PedidoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PedidoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // --- Endpoint 1: Obtener mis pedidos antiguos ---
        [HttpGet]
        public async Task<IActionResult> GetMisPedidos()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var pedidos = await _context.Pedidos
                .Where(p => p.UsuarioId == userId) // <-- TÚ NOMBRE
                .Include(p => p.Detalles) // <-- TÚ NOMBRE
                .OrderByDescending(p => p.Fecha) // <-- TÚ NOMBRE
                .ToListAsync();

            // Mapear a DTOs
            var pedidosDto = pedidos.Select(p => new PedidoResponseDto
            {
                Id = p.Id,
                FechaPedido = p.Fecha.ToString("dd/MM/yyyy hh:mm tt"), // <-- TÚ NOMBRE
                Total = (double)p.Total,
                Items = p.Detalles.Select(dp => new DetallePedidoResponseDto // <-- TÚ NOMBRE
                {
                    ProductoId = dp.ProductoId ?? 0,
                    NombreProducto = dp.NombreProducto,
                    Cantidad = dp.Cantidad,
                    PrecioUnitario = (double)dp.PrecioUnitario
                }).ToList()
            }).ToList();

            return Ok(pedidosDto);
        }


        // --- Endpoint 2: Crear un nuevo pedido (Pagar) ---
        [HttpPost]
        public async Task<IActionResult> CrearPedido()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // 1. Encontrar el carrito del usuario
            var carrito = await _context.Carritos
                .Include(c => c.Items)
                    .ThenInclude(ci => ci.Producto)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (carrito == null || !carrito.Items.Any())
            {
                return BadRequest(new { message = "Tu carrito está vacío." });
            }

            // 2. Crear el objeto Pedido (usando tu modelo)
            var nuevoPedido = new Pedido
            {
                UsuarioId = userId, // <-- TÚ NOMBRE
                Fecha = DateTime.UtcNow, // <-- TÚ NOMBRE
                Total = 0, 
            };

            decimal totalPedido = 0;

            // 3. Convertir CarritoItems en DetallePedidos
            foreach (var item in carrito.Items)
            {
                if (item.Producto == null) 
                    return BadRequest(new { message = $"El producto '{item.ProductoId}' ya no existe."});
                if (item.Producto.Stock < item.Cantidad) 
                    return BadRequest(new { message = $"Stock insuficiente para '{item.Producto.Nombre}'."});

                var detallePedido = new DetallePedido // <-- TÚ NOMBRE
                {
                    Pedido = nuevoPedido,
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.Producto.Precio,
                    NombreProducto = item.Producto.Nombre
                };

                totalPedido += detallePedido.PrecioUnitario * detallePedido.Cantidad;
                _context.DetallePedidos.Add(detallePedido); // <-- TÚ NOMBRE
            }

            nuevoPedido.Total = totalPedido;
            _context.Pedidos.Add(nuevoPedido); 

            // 4. Vaciar el carrito
            _context.CarritoItems.RemoveRange(carrito.Items);

            // 5. Guardar TODOS los cambios
            await _context.SaveChangesAsync();

            return Ok(new { message = "¡Pedido realizado con éxito!", pedidoId = nuevoPedido.Id });
        }
    }
}