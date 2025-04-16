namespace MTS.Web.Models.Curriculum.Quiz.Used
{
    public class StudentAnswerCreateDto
    {
        public string AttemptCode { get; set; }
        public string QuizQuestionCode { get; set; }
        public string SelectedOptionCode { get; set; }
        public string TextAnswer { get; set; }
    }
}