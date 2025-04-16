using System.ComponentModel.DataAnnotations;

namespace MTS.Services.CurriculumAPI.Models.DTO.StudentAnswerDto
{
    public class StudentAnswerCreateDto
    {
        public string AttemptCode { get; set; }

        public string QuizQuestionCode { get; set; }

        // For multiple-choice questions (nullable for text-based)
        public string? SelectedOptionCode { get; set; }

        // For text-based questions (nullable for multiple-choice)
        public string? TextAnswer { get; set; }
    }
}
