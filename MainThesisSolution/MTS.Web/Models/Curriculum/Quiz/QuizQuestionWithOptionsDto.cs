namespace MTS.Web.Models.Curriculum.Quiz
{
    public class QuizQuestionWithOptionsDto
    {
        public QuizQuestionDto Question { get; set; }
        public List<AnswerOptionDto> Options { get; set; } = new List<AnswerOptionDto>();
    }
}
