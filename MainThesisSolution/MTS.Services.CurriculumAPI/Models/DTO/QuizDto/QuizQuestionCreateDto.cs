namespace MTS.Services.CurriculumAPI.Models.DTO.QuizDto
{
    public class QuizQuestionCreateDto
    {
        public string QuizCode { get; set; }
        public string QuestionText { get; set; }
        public int Points { get; set; }
    }
}
