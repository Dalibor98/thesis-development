namespace MTS.Services.CurriculumAPI.Models
{
    public class CourseRegistration
    {
        public int Id { get; set; }
        public string CourseCode{ get; set; }
        public string StudentCode { get; set; }

        //public DateTime RegistrationDate { get; set; } Add if needed later
        public string RegistrationStatus { get; set; } // Active, Dropped, Completed
    }
}
