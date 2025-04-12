namespace MTS.Services.CurriculumAPI.Models
{
    public class StudentAssignmentAttempt
    {
        public int Id { get; set; }
        public string AssignmentCode { get; set; }
        public string StudentUniversityId { get; set; }
        public string SubmissionUrl { get; set; }
        public int Grade { get; set; }
        public int Score { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string SubmissionStatus { get; set; } // Submitted, Graded, Late
        public string Feedback { get; set; }
    }
}
