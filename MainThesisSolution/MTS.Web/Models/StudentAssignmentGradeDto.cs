namespace MTS.Web.Models
{
    public class StudentAssignmentGradeDto
    {
        public int SubmissionId { get; set; }
        public string AssignmentCode { get; set; }
        public int Grade { get; set; }
        public string Feedback { get; set; }
    }
}
