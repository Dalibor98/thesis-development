namespace MTS.Services.CurriculumAPI.Models.DTO.AnswerOption
{
    public class AnswerOptionCreateDto
    {
        public string QuizQuestionCode { get; set; }
        public string OptionText { get; set; }
        public bool IsCorrect { get; set; }
    }
}
