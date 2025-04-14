namespace MTS.Web.Models.Curriculum.Quiz
{
    public class GradeTextQuizViewModel
    {
        public QuizDto Quiz { get; set; }
        public List<AttemptWithStatusViewModel> Attempts { get; set; } = new List<AttemptWithStatusViewModel>();
    }
}
