using ECommerceAPI.Data;
using ECommerceAPI.Models;
using ECommerceAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // ¡LA CLAVE! Solo el rol "Admin" puede acceder.
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context; // ¡Necesitamos el DbContext!

        // Inyectamos UserManager (para usuarios) y ApplicationDbContext (para empresas)
        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpPost("crear-empresa")]
        public async Task<IActionResult> CrearEmpresa([FromBody] RegisterEmpresaRequestDto empresaDto)
        {
            // 1. Verificar si el usuario (email) ya existe
            var userExists = await _userManager.FindByEmailAsync(empresaDto.Email);
            if (userExists != null)
            {
                return BadRequest(new { Message = "El email ya está en uso." });
            }

            // 2. Crear el nuevo ApplicationUser
            var newUser = new ApplicationUser
            {
                UserName = empresaDto.Username,
                Email = empresaDto.Email,
                EmailConfirmed = true // El Admin los confirma automáticamente
            };

            var result = await _userManager.CreateAsync(newUser, empresaDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { Message = "Error al crear el usuario", Errors = result.Errors });
            }

            // 3. Asignar el rol "Empresa"
            var roleResult = await _userManager.AddToRoleAsync(newUser, "Empresa");
            if (!roleResult.Succeeded)
            {
                // Si falla asignar el rol, borramos el usuario creado (Rollback)
                await _userManager.DeleteAsync(newUser);
                return BadRequest(new { Message = "Error al asignar el rol 'Empresa'", Errors = roleResult.Errors });
            }

            // 4. Crear la entidad "Empresa"
            var nuevaEmpresa = new Empresa
            {
                Nombre = empresaDto.NombreEmpresa,
                Descripcion = empresaDto.DescripcionEmpresa ?? "", // Asignar "" si es nulo
                UserId = newUser.Id // ¡Vincular la empresa con el usuario recién creado!
            };

            // 5. Guardar la empresa en la base de datos
            _context.Empresas.Add(nuevaEmpresa);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Si falla guardar la empresa, borramos el usuario (Rollback)
                await _userManager.DeleteAsync(newUser);
                return StatusCode(500, new { Message = "Error al guardar la empresa en la base de datos", Error = ex.Message });
            }

            return Ok(new { Message = "Usuario y Empresa creados exitosamente." });
        }
    }
}