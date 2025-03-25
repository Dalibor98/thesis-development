namespace MTS.Web.Models
{
    public class ProfessorCreateDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string UniversityId { get; set; } // Required for verification
        public string Department { get; set; }
        public string Title { get; set; }
    }
}
