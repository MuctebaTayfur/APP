using APP.Auth.Model.Entity;
using APP.Base.Model.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace APP.Auth.Model
{
   public class AuthContext:IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Packet> Packets { get; set; }
        public DbSet<ApplicationUserProduct> ApplicationUserProducts { get; set; }


        public AuthContext(DbContextOptions<AuthContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                relationship.DeleteBehavior = DeleteBehavior.Restrict;

            modelBuilder.HasDefaultSchema("auth");
            modelBuilder.Entity<ApplicationUserProduct>()
                .HasKey(up => new { up.ApplicationUserId,up.ProductId,up.CompanyId});
            modelBuilder.Entity<ApplicationUserProduct>()
                .HasOne(up => up.ApplicationUser)
                .WithMany(au => au.ApplicationUserProducts)
                .HasForeignKey(au => au.ApplicationUserId);
            modelBuilder.Entity<ApplicationUserProduct>()
                .HasOne(up => up.Product)
                .WithMany(p => p.ApplicationUserProducts)
                .HasForeignKey(au => au.ProductId);
            modelBuilder.Entity<ApplicationUserProduct>()
              .HasOne(up => up.Company)
              .WithMany(p => p.ApplicationUserProducts)
              .HasForeignKey(au => au.CompanyId);

        }
    }
}
