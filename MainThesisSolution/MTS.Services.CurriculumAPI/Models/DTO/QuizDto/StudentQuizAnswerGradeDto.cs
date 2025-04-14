namespace MTS.Services.CurriculumAPI.Models.DTO.QuizDto
{
    public class StudentQuizAnswerGradeDto
    {
        public int Id { get; set; }
        public bool IsCorrect { get; set; }
        public int PointsEarned { get; set; }
    }
}
