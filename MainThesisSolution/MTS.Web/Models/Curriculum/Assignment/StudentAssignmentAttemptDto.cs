namespace MTS.Web.Models.Curriculum.Assignment
{
    public class StudentAssignmentAttemptDto
    {
        public int Id { get; set; }
        public string AssignmentCode { get; set; }
        public string StudentUniversityId { get; set; }
        public string SubmissionUrl { get; set; }
        public int Grade { get; set; }
        public int Score { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string SubmissionStatus { get; set; }
        public string Feedback { get; set; }
    }
}
