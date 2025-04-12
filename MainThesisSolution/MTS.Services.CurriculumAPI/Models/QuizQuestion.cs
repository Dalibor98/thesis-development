using Microsoft.VisualBasic.FileIO;

namespace MTS.Services.CurriculumAPI.Models
{
    public class QuizQuestion
    {
        public int Id { get; set; }
        public string QuizCode { get; set; }
        public string QuizQuestionCode { get; set; }
        public string QuestionText { get; set; }
        public int Points { get; set; }
    }
}
