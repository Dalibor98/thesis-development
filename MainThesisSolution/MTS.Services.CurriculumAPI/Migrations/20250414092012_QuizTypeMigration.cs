using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTS.Services.CurriculumAPI.Migrations
{
    /// <inheritdoc />
    public partial class QuizTypeMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QuizType",
                table: "Quizzes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuizType",
                table: "Quizzes");
        }
    }
}
