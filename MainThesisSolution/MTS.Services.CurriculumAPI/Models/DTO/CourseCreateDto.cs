namespace MTS.Services.CurriculumAPI.Models.DTO
{
    public class CourseCreateDto
    {
        public string Title { get; set; }
        public string CourseCode { get; set; }
        public string Description { get; set; }
        public string ProfessorUniversityId { get; set; }

    }
}
