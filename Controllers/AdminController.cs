using ECommerceAPI.Data;
using ECommerceAPI.Models;
using ECommerceAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Protegemos todo el controlador para que SOLO el Admin pueda usarlo
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpPost("crear-empresa")]
        public async Task<IActionResult> CrearEmpresa([FromBody] CrearEmpresaDto dto)
        {
            // 1. Validar que el email no exista
            var userExists = await _userManager.FindByEmailAsync(dto.Email);
            if (userExists != null)
            {
                return BadRequest(new { message = "El email ya está registrado." });
            }

            // 2. Crear el ApplicationUser
            var newUser = new ApplicationUser()
            {
                Email = dto.Email,
                UserName = dto.Email, // Usamos el email como UserName
                SecurityStamp = Guid.NewGuid().ToString()
                // Nota: No guardamos Nombre/Apellido aquí, 
                // esos son para usuarios "Cliente"
            };

            var result = await _userManager.CreateAsync(newUser, dto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new { 
                    message = "Error al crear el usuario.", 
                    errors = result.Errors 
                });
            }

            // 3. Asignar el rol "Empresa"
            await _userManager.AddToRoleAsync(newUser, "Empresa");

            // 4. Crear la entidad Empresa y ligarla al usuario
            var nuevaEmpresa = new Empresa
            {
                Nombre = dto.Nombre,
                Rubro = dto.Rubro,
                FechaRegistro = DateTime.UtcNow,
                // ¡Esta es la conexión clave!
                ApplicationUserId = newUser.Id 
            };

            _context.Empresas.Add(nuevaEmpresa);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Empresa registrada exitosamente!" });
        }
    }
}

