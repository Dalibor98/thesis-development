namespace MTS.Web.Models.Curriculum.Quiz
{
    public class QuizQuestionDto
    {
        public int Id { get; set; }
        public string QuizCode { get; set; }
        public string QuizQuestionCode { get; set; }
        public string QuestionText { get; set; }
        public int Points { get; set; }
    }
}
