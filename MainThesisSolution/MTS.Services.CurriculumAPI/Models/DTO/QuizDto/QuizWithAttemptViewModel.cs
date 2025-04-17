namespace MTS.Services.CurriculumAPI.Models.DTO.QuizDto
{
    public class QuizWithAttemptsViewModel
    {
        public QuizDto Quiz { get; set; }
        public List<StudentQuizAttemptDto> PendingAttempts { get; set; } = new List<StudentQuizAttemptDto>();
    }
}
