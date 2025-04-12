using System.ComponentModel.DataAnnotations;

namespace MTS.Web.Models.Curriculum.Week
{
    public class WeekCreateDto
    {
        [Required]
        public string CourseCode { get; set; }
        // No other properties
    }
}
