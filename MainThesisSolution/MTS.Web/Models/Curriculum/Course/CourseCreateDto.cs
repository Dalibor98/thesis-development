using System.ComponentModel.DataAnnotations;

namespace MTS.Web.Models.Curriculum.Course
{
    public class CourseCreateDto
    {
        [Required]
        [Display(Name = "Course Title")]
        public string Title { get; set; }

        public string CourseCode { get; set; }

        [Required]
        [Display(Name = "Course Description")]
        public string Description { get; set; }

        public string ProfessorUniversityId { get; set; }
    }
}
