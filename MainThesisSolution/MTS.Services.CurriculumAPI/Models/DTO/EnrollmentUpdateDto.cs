namespace MTS.Services.CurriculumAPI.Models.DTO
{
    public class EnrollmentUpdateDto
    {
        public int Id { get; set; }
        public string RegistrationStatus { get; set; } // Active, Dropped, Completed
    }
}
