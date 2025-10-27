using ECommerceAPI.Models; // Para Empresa, Producto, Carrito
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Data
{
    // ¡Importante! Hereda de IdentityDbContext<ApplicationUser>
    // para usar nuestra clase de usuario personalizada.
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // --- Tablas de nuestra App ---
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<CarritoItem> CarritoItems { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            // --- Configuración de Identity ---
            base.OnModelCreating(builder); // <- ¡Siempre primero!

            
            // --- Relaciones (Pasos 3 y 4) ---

            // Relación 1-a-1: Empresa <-> ApplicationUser
            // Un usuario (con rol Empresa) gestiona una Empresa
            builder.Entity<Empresa>()
                .HasOne(e => e.User) // Una Empresa tiene un User
                .WithMany() // Un User puede (en teoría) tener muchas empresas, aunque no lo usemos
                .HasForeignKey(e => e.UserId); // La clave foránea está en Empresa

            // Relación 1-a-Muchos: Empresa -> Producto
            // Una Empresa tiene muchos Productos
            builder.Entity<Empresa>()
                .HasMany(e => e.Productos) // Una Empresa tiene muchos Productos
                .WithOne(p => p.Empresa) // Un Producto pertenece a una Empresa
                .HasForeignKey(p => p.EmpresaId); // La clave foránea está en Producto


            // --- Relaciones del Carrito (Paso 6) ---

            // Relación 1-a-1: ApplicationUser <-> Carrito
            // Un Usuario tiene un Carrito
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Carrito) // Un User tiene un Carrito
                .WithOne(c => c.User) // Un Carrito pertenece a un User
                .HasForeignKey<Carrito>(c => c.UserId); // La clave foránea está en Carrito

            // Relación 1-a-Muchos: Carrito -> CarritoItem
            // Un Carrito tiene muchos Items
            builder.Entity<Carrito>()
                .HasMany(c => c.Items) // Un Carrito tiene muchos Items
                .WithOne(ci => ci.Carrito) // Un Item pertenece a un Carrito
                .HasForeignKey(ci => ci.CarritoId); // La clave foránea está en CarritoItem

            // Relación 1-a-Muchos: Producto -> CarritoItem
            // Un Producto puede estar en muchos Items de carrito
            builder.Entity<Producto>()
                .HasMany<CarritoItem>() // Un Producto puede estar en muchos CarritoItems
                .WithOne(ci => ci.Producto) // Un Item tiene un Producto
                .HasForeignKey(ci => ci.ProductoId); // La clave foránea está en CarritoItem
        }
    }
}
