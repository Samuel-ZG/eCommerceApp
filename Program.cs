using System.Text;
using ECommerceAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; // Para Swagger

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// --- 1. Conectar la Base de Datos (DbContext) ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

// --- 2. Configurar Identity (Usuarios y Roles) ---
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// --- 3. Configurar Autenticación con Tokens JWT ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // En desarrollo; cambiar a true en producción
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = configuration["Jwt:Audience"],
        ValidIssuer = configuration["Jwt:Issuer"],
        // Esta es la validación de la firma (la clave secreta)
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
    };
});

// --- 4. Añadir Servicios ---
builder.Services.AddControllers();

// --- 5. Configurar Swagger (La documentación de tu API) ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Esto añade el botón "Authorize" en Swagger para probar endpoints protegidos
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ECommerceAPI", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa 'Bearer' [espacio] y luego tu token. Ejemplo: 'Bearer 12345abcdef'"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// --- Construir la App ---
var app = builder.Build();

// --- AÑADIR ESTE BLOQUE PARA SEMBRAR DATOS (SEED) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Obtiene los servicios de Identity
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Llama a nuestro inicializador estático
        await ECommerceAPI.Data.DbInitializer.Initialize(userManager, roleManager);
    }
    catch (Exception ex)
    {
        // Si algo sale mal, lo registra en la consola
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Un error ocurrió al sembrar la base de datos.");
    }
}
// --- FIN DEL BLOQUE AÑADIDO ---

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- ¡ORDEN IMPORTANTE! ---
// 1. app.UseAuthentication(): Verifica QUIÉN eres (lee el token).
app.UseAuthentication();
// 2. app.UseAuthorization(): Verifica QUÉ puedes hacer (mira tu Rol).
app.UseAuthorization();
// ---------------------------

app.MapControllers();

app.Run();