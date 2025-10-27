using ECommerceAPI.Data;
using ECommerceAPI.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        // Inyectamos los servicios que necesitamos
        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerDto)
        {
            // 1. Verificar si el usuario ya existe
            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
            {
                return BadRequest(new AuthResponseDto { IsSuccess = false, Message = "El email ya está en uso." });
            }

            // 2. Crear el nuevo usuario
            var newUser = new ApplicationUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };

            var result = await _userManager.CreateAsync(newUser, registerDto.Password);

            if (!result.Succeeded)
            {
                // Si falla la creación, devuelve los errores
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new AuthResponseDto { IsSuccess = false, Message = errors });
            }

            // 3. Asignar el rol por defecto: "Usuario"
            await _userManager.AddToRoleAsync(newUser, "Usuario");

            return Ok(new AuthResponseDto { IsSuccess = true, Message = "¡Usuario registrado exitosamente!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            // 1. Encontrar al usuario
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            // 2. Si existe y la contraseña es correcta
            if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                // 3. Generar el Token JWT
                var token = await GenerateJwtToken(user);
                var roles = await _userManager.GetRolesAsync(user);

                return Ok(new AuthResponseDto
                {
                    IsSuccess = true,
                    Message = "Login exitoso",
                    Token = token,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            // 3. Si falla
            return Unauthorized(new AuthResponseDto { IsSuccess = false, Message = "Email o contraseña incorrectos." });
        }

        // --- Método privado para generar el Token ---
        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            // El "payload" del token. Contiene la info del usuario (claims).
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id), // ID del usuario
                new Claim(ClaimTypes.Email, user.Email),       // Email
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // ID único del token
            };

            // Añadir los roles del usuario a los claims
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Obtener la clave secreta y la configuración del appsettings.json
            var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            // Crear el token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(24), // Duración del token
                SigningCredentials = new SigningCredentials(jwtKey, SecurityAlgorithms.HmacSha256),
                Issuer = issuer,
                Audience = audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}