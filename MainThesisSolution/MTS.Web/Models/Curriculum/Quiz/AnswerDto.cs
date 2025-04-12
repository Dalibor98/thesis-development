namespace MTS.Web.Models.Curriculum.Quiz
{
    public class AnswerDto
    {
        public int Id { get; set; }
        public string QuizQuestionCode { get; set; }
        public string AnswerCode { get; set; }
        public string OptionText { get; set; }
        public bool IsCorrect { get; set; }
    }
}
