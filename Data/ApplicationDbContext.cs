using Microsoft.EntityFrameworkCore;
using BaseApi.Models;

namespace BaseApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuraciones adicionales para el modelo User
            modelBuilder.Entity<User>(entity =>
            {
                // Índice único para Username
                entity.HasIndex(u => u.Username)
                      .IsUnique()
                      .HasDatabaseName("IX_Users_Username");

                // Índice único para Email
                entity.HasIndex(u => u.Email)
                      .IsUnique()
                      .HasDatabaseName("IX_Users_Email");

                // Configuración de columnas
                entity.Property(u => u.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(u => u.IsActive)
                      .HasDefaultValue(true);

                // Configuración de precision para DateTime
                entity.Property(u => u.CreatedAt)
                      .HasColumnType("datetime2");

                entity.Property(u => u.UpdatedAt)
                      .HasColumnType("datetime2");

                entity.Property(u => u.LastLoginAt)
                      .HasColumnType("datetime2");
            });
        }
    }
}