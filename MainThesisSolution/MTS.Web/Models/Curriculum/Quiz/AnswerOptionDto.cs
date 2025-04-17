namespace MTS.Web.Models.Curriculum.Quiz
{
    public class AnswerOptionDto
    {
        public int Id { get; set; }
        public string QuizQuestionCode { get; set; }
        public string OptionCode { get; set; }
        public string OptionText { get; set; }
        public bool IsCorrect { get; set; }
    }
}
