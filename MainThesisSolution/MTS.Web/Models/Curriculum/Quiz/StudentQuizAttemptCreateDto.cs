namespace MTS.Web.Models.Curriculum.Quiz
{
    public class StudentQuizAttemptCreateDto
    {
        public string QuizCode { get; set; }
        public string StudentUniversityId { get; set; }
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; }
        public int Score { get; set; } = 0;
    }
}
