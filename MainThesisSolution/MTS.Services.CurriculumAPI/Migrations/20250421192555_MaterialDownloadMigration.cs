using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTS.Services.CurriculumAPI.Migrations
{
    /// <inheritdoc />
    public partial class MaterialDownloadMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                table: "Materials",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileUrl",
                table: "Materials");
        }
    }
}
