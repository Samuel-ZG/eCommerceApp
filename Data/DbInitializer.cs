using ECommerceAPI.Data;
using Microsoft.AspNetCore.Identity;

namespace ECommerceAPI.Data
{
    public static class DbInitializer
    {
        // Método de inicialización
        public static async Task Initialize(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // --- 1. Definir los Roles ---
            string[] roleNames = { "Admin", "Empresa", "Usuario" };

            foreach (var roleName in roleNames)
            {
                // ¿Existe el rol?
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    // Si no existe, lo crea
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // --- 2. Crear el Usuario Admin ---

            // Busca si ya existe un usuario con este email
            var adminUser = await userManager.FindByEmailAsync("admin@admin.com");

            if (adminUser == null)
            {
                // Si no existe, crea el nuevo usuario Admin
                var newAdminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@admin.com",
                    EmailConfirmed = true // Confirma el email automáticamente
                };

                // ¡IMPORTANTE! Cambia "Admin123!" por una contraseña segura
                var result = await userManager.CreateAsync(newAdminUser, "Admin123!");

                if (result.Succeeded)
                {
                    // Asigna el rol "Admin" al usuario recién creado
                    await userManager.AddToRoleAsync(newAdminUser, "Admin");
                }
            }
        }
    }
}