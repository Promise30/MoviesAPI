using Microsoft.EntityFrameworkCore;
using MoviesAPI.Models;

namespace MoviesAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>()
                .Property(m => m.Rating)
                .HasColumnType("decimal(3, 1)"); // Specifies the SQL Server column type as decimal(3, 1)

            // Other configurations for your entities

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Movie> Movies { get; set; }
    }
}
