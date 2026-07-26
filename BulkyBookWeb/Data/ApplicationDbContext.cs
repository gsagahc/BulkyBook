using Microsoft.EntityFrameworkCore;
using BulkyBookWeb.Models;
namespace BulkyBookWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Define your database tables here
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1, OrderID = 0 },
                new Category { Id = 2, Name = "SciFi" , DisplayOrder = 2, OrderID = 1 },
                new Category { Id = 3, Name = "History" , DisplayOrder = 3, OrderID = 2 }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
  
