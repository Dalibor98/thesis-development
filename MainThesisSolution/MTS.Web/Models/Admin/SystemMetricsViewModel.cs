namespace MTS.Web.Models.Admin
{
    public class SystemMetricsViewModel
    {
        public int StudentCount { get; set; }
        public int ProfessorCount { get; set; }
        public int CourseCount { get; set; }
        public int ActiveEnrollmentCount { get; set; }
        public int TotalUserCount => StudentCount + ProfessorCount;
    }
}
