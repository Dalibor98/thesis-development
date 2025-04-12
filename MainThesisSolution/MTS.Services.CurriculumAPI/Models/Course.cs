namespace MTS.Services.CurriculumAPI.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CourseCode { get; set; }
        public string Description { get; set; }
        public string ProfessorUniversityId { get; set; }    
    }
}

