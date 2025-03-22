namespace MTS.Services.UserAPI.Models.DTO
{
    public class StudentCreateDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string UniversityId { get; set; } 
        public string Major { get; set; }
        public int EnrollmentYear { get; set; }

    }
}
