namespace MTS.Services.CurriculumAPI.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        //build a function that creates a course codes.
        public string CourseCode { get; set; }
        public string Description { get; set; }

        //UniversityId of the professor
        public string ProfessorUniversityId { get; set; }
       
    }
}

