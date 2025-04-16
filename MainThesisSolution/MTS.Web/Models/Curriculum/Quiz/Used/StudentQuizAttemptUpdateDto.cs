namespace MTS.Web.Models.Curriculum.Quiz.Used
{
    public class StudentQuizAttemptUpdateDto
    {
        public int Id { get; set; }
        public string QuizCode { get; set; }
        public string AttemptCode { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime StartTime { get; set; }
        public string StudentUniversityId { get; set; }

        public int Score { get; set; }
        // Note: Quiz code and student ID shouldn't be updatable
        // so they're intentionally excluded from this DTO
    }
}
