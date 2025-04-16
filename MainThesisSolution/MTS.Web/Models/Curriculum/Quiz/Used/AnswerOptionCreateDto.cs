namespace MTS.Web.Models.Curriculum.Quiz.Used
{
    public class AnswerOptionCreateDto
    {
        public string QuizQuestionCode { get; set; }
        public string OptionText { get; set; }
        public bool IsCorrect { get; set; }
    }
}