namespace MTS.Web.Models.Curriculum.Assignment
{
    public class StudentAssignmentAttemptCreateDto
    {
        public string AssignmentCode { get; set; }
        public string StudentUniversityId { get; set; }
        public string SubmissionUrl { get; set; }
        public string Comments { get; set; }
    }
}
