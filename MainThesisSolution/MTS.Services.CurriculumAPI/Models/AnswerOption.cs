namespace MTS.Services.CurriculumAPI.Models
{
    public class AnswerOption
    {
        public int Id { get; set; }
        public string QuizQuestionCode { get; set; }
        public string OptionCode { get; set; }
        public string OptionText { get; set; }
        public bool IsCorrect { get; set; }
    }
}
