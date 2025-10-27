using ECommerceAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Data
{
    // Hereda de IdentityDbContext para incluir las tablas de Usuarios y Roles
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // --- Tablas de E-Commerce ---
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallesPedido { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Llama al OnModelCreating de Identity para que configure sus tablas
            base.OnModelCreating(builder);

            // --- Configuraciones de Relaciones (E-Commerce) ---

            // Relación Usuario -> Empresa (Un Usuario tiene Muchas Empresas)
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Empresas)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId);

            // Relación Empresa -> Producto (Una Empresa tiene Muchos Productos)
            builder.Entity<Empresa>()
                .HasMany(e => e.Productos)
                .WithOne(p => p.Empresa)
                .HasForeignKey(p => p.EmpresaId);

            // Relación Usuario -> Pedido (Un Usuario tiene Muchos Pedidos)
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Pedidos)
                .WithOne(p => p.Usuario)
                .HasForeignKey(p => p.UsuarioId);

            // Relación Pedido -> DetallePedido (Un Pedido tiene Muchos Detalles)
            builder.Entity<Pedido>()
                .HasMany(p => p.Detalles)
                .WithOne(d => d.Pedido)
                .HasForeignKey(d => d.PedidoId);

            // Relación Producto -> DetallePedido (Un Producto puede estar en Muchos Detalles)
            builder.Entity<Producto>()
                .HasMany<DetallePedido>()
                .WithOne(d => d.Producto)
                .HasForeignKey(d => d.ProductoId);
        }
    }
}
