namespace MTS.Web.Models.Curriculum.Quiz
{
    public class StudentAnswerDto
    {
        public int Id { get; set; }
        public string AttemptCode { get; set; }
        public string QuizQuestionCode { get; set; }
        public string SelectedOptionCode { get; set; }
        public string TextAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public int PointsEarned { get; set; }
        public string GradingStatus { get; set; }
    }
}
