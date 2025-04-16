using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTS.Services.CurriculumAPI.Migrations
{
    /// <inheritdoc />
    public partial class AnswerQuestionMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "StudentQuizAnswers");

            migrationBuilder.AddColumn<string>(
                name: "QuestionType",
                table: "QuizQuestions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AnswerOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuizQuestionCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerOptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttemptCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuizQuestionCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SelectedOptionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TextAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    PointsEarned = table.Column<int>(type: "int", nullable: false),
                    GradingStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAnswers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerOptions");

            migrationBuilder.DropTable(
                name: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "QuestionType",
                table: "QuizQuestions");

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnswerCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    OptionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuizQuestionCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentQuizAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnswerCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttemptCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    PointsEarned = table.Column<int>(type: "int", nullable: false),
                    QuizQuestionCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TextAnswer = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentQuizAnswers", x => x.Id);
                });
        }
    }
}
