using CBS.Services.ProductAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CBS.Services.ProductAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 1,
                Name = "Batman:Hush",
                Price = 15,
                Description = "Gotham City is infected by a crime epidemic and all of Batman's enemies have emerged to throw his life into utter chaos. But little do they know, they're all pawns of the villainous Hush in an elaborate game of revenge against Bruce Wayne. Pushed past his breaking point, Batman will need to use more than the world's greatest detective skills to uncover the true identity of this mysterious mastermind before it's too late.",
                ImageUrl = "https://m.media-amazon.com/images/I/91TffnqrdeL._SL1500_.jpg",
                CategoryName = "BatmanComics"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 2,
                Name = "Batman: The Long Halloween",
                Price = 13.99,
                Description = "Working with District Attorney Harvey Dent and Lieutenant James Gordon, Batman races against the calendar as he tries to discover who Holiday is before he claims his next victim each month. A mystery that has the reader continually guessing the identity of the killer, this story also ties into the events that transform Harvey Dent into Batman's deadly enemy, Two-Face.",
                ImageUrl = "https://m.media-amazon.com/images/I/81oxI3fwvlL._SL1500_.jpg",
                CategoryName = "BatmanComics"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 3,
                Name = "All-Star Superman",
                Price = 10.99,
                Description = "ADVENTURES OF SUPERMAN! Topsy-turvy madness on the backwards Bizarro planet. A bottled city that proves you can never go home again. A living sun hell-bent on destroying humanity. A world without the Man of Steel. Twelve impossible labors and mere moments to save the Earth.",
                ImageUrl = "https://m.media-amazon.com/images/I/81IlLHwDXuL._SL1500_.jpg",
                CategoryName = "SupermanComics"
            });
        }
    }
    

}
