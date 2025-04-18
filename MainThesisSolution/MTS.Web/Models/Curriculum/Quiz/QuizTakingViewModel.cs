namespace MTS.Web.Models.Curriculum.Quiz
{
    public class QuizTakingViewModel
    {
        public QuizDto Quiz { get; set; }
        public List<QuizQuestionWithOptionsDto> Questions { get; set; }
        public StudentQuizAttemptDto Attempt { get; set; }
    }
}
