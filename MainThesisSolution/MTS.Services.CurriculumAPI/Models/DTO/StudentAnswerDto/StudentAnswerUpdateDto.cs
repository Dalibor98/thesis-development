using System.ComponentModel.DataAnnotations;

namespace MTS.Services.CurriculumAPI.Models.DTO.StudentAnswerDto
{
    public class StudentAnswerUpdateDto
    {
        public int Id { get; set; }

        // For multiple-choice questions
        public string? SelectedOptionCode { get; set; }

        // For text-based questions
        public string? TextAnswer { get; set; }

        // These fields can be updated by professors during grading
        public bool IsCorrect { get; set; }
        public int PointsEarned { get; set; }
        public string GradingStatus { get; set; } = "Ungraded";
    }
}
