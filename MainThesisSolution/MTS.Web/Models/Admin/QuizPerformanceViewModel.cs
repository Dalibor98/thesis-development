namespace MTS.Web.Models.Admin
{
    public class QuizPerformanceViewModel
    {
        public string QuizCode { get; set; }
        public string QuizTitle { get; set; }
        public string CourseCode { get; set; }
        public string CourseTitle { get; set; }
        public int TotalEnrolledStudents { get; set; }
        public int StudentsAttempted { get; set; }
        public double AttemptPercentage =>
            TotalEnrolledStudents > 0
            ? Math.Round((double)StudentsAttempted / TotalEnrolledStudents * 100, 2) : 0;
        public double AverageScore { get; set; }
    }
}
