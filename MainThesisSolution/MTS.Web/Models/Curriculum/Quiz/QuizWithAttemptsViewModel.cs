namespace MTS.Web.Models.Curriculum.Quiz
{
    public class QuizWithAttemptsViewModel
    {
        public QuizDto Quiz { get; set; }
        public List<StudentQuizAttemptDto> PendingAttempts { get; set; } = new List<StudentQuizAttemptDto>();
    }
}
