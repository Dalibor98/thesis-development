namespace MTS.Services.CurriculumAPI.Models
{
    public class Week
    {
        public int Id { get; set; }
        public string CourseCode { get; set; }
        public string WeekCode { get; set; }
        public int WeekNumber { get; set; }

        /*
         * public static string GenerateWeekCode(string courseCode, int weekNumber)
        {
            return $"{courseCode}-W{weekNumber:D2}";
        }
         */
    }
}
