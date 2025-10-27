using ECommerceAPI.Models; // Para Empresa, Producto, Carrito, Pedido
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
        
        // --- ¡ESTAS SON LAS LÍNEAS QUE FALTABAN! ---
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallePedidos { get; set; }
        // --- FIN DE LA CORRECCIÓN ---
        


        protected override void OnModelCreating(ModelBuilder builder)
        {
            // --- Configuración de Identity ---
            base.OnModelCreating(builder); // <- ¡Siempre primero!

            
            // --- Relaciones (Pasos 3 y 4) ---

            // Relación 1-a-1: Empresa <-> ApplicationUser
            builder.Entity<Empresa>()
                .HasOne(e => e.ApplicationUser) 
                .WithMany() 
                .HasForeignKey(e => e.ApplicationUserId); 

            // Relación 1-a-Muchos: Empresa -> Producto
            builder.Entity<Empresa>()
                .HasMany(e => e.Productos) 
                .WithOne(p => p.Empresa) 
                .HasForeignKey(p => p.EmpresaId); 


            // --- Relaciones del Carrito (Paso 6) ---

            // Relación 1-a-1: ApplicationUser <-> Carrito
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Carrito) 
                .WithOne(c => c.User) 
                .HasForeignKey<Carrito>(c => c.UserId); 

            // Relación 1-a-Muchos: Carrito -> CarritoItem
            builder.Entity<Carrito>()
                .HasMany(c => c.Items) 
                .WithOne(ci => ci.Carrito) 
                .HasForeignKey(ci => ci.CarritoId); 

            // Relación 1-a-Muchos: Producto -> CarritoItem
            builder.Entity<Producto>()
                .HasMany<CarritoItem>() 
                .WithOne(ci => ci.Producto) 
                .HasForeignKey(ci => ci.ProductoId); 
                
            // --- Relaciones de Pedidos (CON TUS NOMBRES) ---

            // 1-a-Muchos: Usuario -> Pedido
            builder.Entity<ApplicationUser>()
                .HasMany<Pedido>() // <-- Tu modelo Pedido
                .WithOne(p => p.Usuario) // <-- Tu propiedad Usuario
                .HasForeignKey(p => p.UsuarioId); // <-- Tu propiedad UsuarioId

            // 1-a-Muchos: Pedido -> DetallePedido
            builder.Entity<Pedido>()
                .HasMany(p => p.Detalles) // <-- Tu propiedad Detalles
                .WithOne(dp => dp.Pedido)
                .HasForeignKey(dp => dp.PedidoId);

            // 1-a-Muchos: Producto -> DetallePedido
            builder.Entity<Producto>()
                .HasMany<DetallePedido>() // <-- Tu modelo DetallePedido
                .WithOne(dp => dp.Producto)
                .HasForeignKey(dp => dp.ProductoId)
                .OnDelete(DeleteBehavior.SetNull); // <-- ¡La clave!
        }
    }
}