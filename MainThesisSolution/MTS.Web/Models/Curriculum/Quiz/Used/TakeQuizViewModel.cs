namespace MTS.Web.Models.Curriculum.Quiz.Used
{
    public class TakeQuizViewModel
    {
        public StudentQuizAttemptDto Attempt { get; set; }
        public QuizDto Quiz { get; set; }
        public QuizQuestionDto Question { get; set; }
        public List<AnswerOptionDto> AnswerOptions { get; set; }
        public StudentAnswerDto StudentAnswer { get; set; }
        public int QuestionIndex { get; set; }
        public int TotalQuestions { get; set; }
        public int TimeRemaining { get; set; } // In seconds
    }
}
