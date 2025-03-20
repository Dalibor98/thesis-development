using CBS.Services.PlumAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CBS.Services.PlumAPI.Data
{
    public class PlumDbContext : DbContext
    {
        public PlumDbContext(DbContextOptions<PlumDbContext> options) : base(options)
        {
        }

        public DbSet<Plum> Plums { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Plum>().HasData(
                new Plum
                {
                    PlumId = 1,
                    Name = "Plum Variety A",
                    Price = 5.99,
                    Description = "A sweet and tangy plum variety.",
                    CategoryName = "FreshFruits"
                },
                new Plum
                {
                    PlumId = 2,
                    Name = "Plum Variety B",
                    Price = 6.49,
                    Description = "A rich and flavorful plum.",
                    CategoryName = "FreshFruits"
                }
            );
        }
    }
}
