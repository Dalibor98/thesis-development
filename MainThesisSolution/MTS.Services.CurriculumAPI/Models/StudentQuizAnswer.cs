namespace MTS.Services.CurriculumAPI.Models
{
    public class StudentQuizAnswer
    {
        public int Id { get; set; }
        public string AttemptCode { get; set; }
        public string QuizQuestionCode { get; set; }
        public string AnswerCode { get; set; }
        public string TextAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public int PointsEarned { get; set; }
    }
}
