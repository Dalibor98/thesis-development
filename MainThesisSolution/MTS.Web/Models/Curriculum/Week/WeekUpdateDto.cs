using System.ComponentModel.DataAnnotations;

namespace MTS.Web.Models.Curriculum.Week
{
    public class WeekUpdateDto
    {
        [Required]
        public string WeekCode { get; set; }

        [Required]
        [Range(1, 52, ErrorMessage = "Week number must be between 1 and 52")]
        public int WeekNumber { get; set; }

        // CourseCode for reference but it shouldn't be modifiable
        public string CourseCode { get; set; }
    }
}
