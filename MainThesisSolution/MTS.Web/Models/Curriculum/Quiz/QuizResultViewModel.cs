namespace MTS.Web.Models.Curriculum.Quiz
{
    public class QuizResultViewModel
    {
        public StudentQuizAttemptDto Attempt { get; set; }
        public QuizDto Quiz { get; set; }
        public List<QuizQuestionDto> Questions { get; set; }
        public List<StudentQuizAnswerDto> StudentAnswers { get; set; }
    }
}
