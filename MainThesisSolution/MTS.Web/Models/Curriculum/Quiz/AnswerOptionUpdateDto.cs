namespace MTS.Web.Models.Curriculum.Quiz
{
    public class AnswerOptionUpdateDto
    {
        public int Id { get; set; }
        public string QuizQuestionCode { get; set; }
        public string OptionText { get; set; }
        public bool IsCorrect { get; set; }
    }

}