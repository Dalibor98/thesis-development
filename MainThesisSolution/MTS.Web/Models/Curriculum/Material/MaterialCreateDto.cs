using System.ComponentModel.DataAnnotations;

namespace MTS.Web.Models.Curriculum.Material
{
    public class MaterialCreateDto
    {
        public string CourseCode { get; set; }
        public string WeekCode { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Material Type")]
        public string MaterialType { get; set; } // Video, PDF, Link, etc.
    }
}
