namespace MTS.Web.Models.Curriculum.Quiz
{
    public class StudentQuizAttemptUpdateDto
    {
        public int Id { get; set; }
        public string AttemptCode { get; set; }
        public DateTime EndTime { get; set; }
        public int Score { get; set; }
    }
}
