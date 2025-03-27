using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTS.Services.AuthAPI.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UniversityId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UniversityId",
                table: "AspNetUsers");
        }
    }
}
