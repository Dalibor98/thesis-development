using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CBS.Services.ProductAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialProductMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageLocalPath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryName", "Description", "ImageLocalPath", "ImageUrl", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "BatmanComics", "Gotham City is infected by a crime epidemic and all of Batman's enemies have emerged to throw his life into utter chaos. But little do they know, they're all pawns of the villainous Hush in an elaborate game of revenge against Bruce Wayne. Pushed past his breaking point, Batman will need to use more than the world's greatest detective skills to uncover the true identity of this mysterious mastermind before it's too late.", null, "https://m.media-amazon.com/images/I/91TffnqrdeL._SL1500_.jpg", "Batman:Hush", 15.0 },
                    { 2, "BatmanComics", "Working with District Attorney Harvey Dent and Lieutenant James Gordon, Batman races against the calendar as he tries to discover who Holiday is before he claims his next victim each month. A mystery that has the reader continually guessing the identity of the killer, this story also ties into the events that transform Harvey Dent into Batman's deadly enemy, Two-Face.", null, "https://m.media-amazon.com/images/I/81oxI3fwvlL._SL1500_.jpg", "Batman: The Long Halloween", 13.99 },
                    { 3, "SupermanComics", "ADVENTURES OF SUPERMAN! Topsy-turvy madness on the backwards Bizarro planet. A bottled city that proves you can never go home again. A living sun hell-bent on destroying humanity. A world without the Man of Steel. Twelve impossible labors and mere moments to save the Earth.", null, "https://m.media-amazon.com/images/I/81IlLHwDXuL._SL1500_.jpg", "All-Star Superman", 10.99 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
