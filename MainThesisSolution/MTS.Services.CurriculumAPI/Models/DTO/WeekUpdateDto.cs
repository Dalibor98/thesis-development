using System.ComponentModel.DataAnnotations;

namespace MTS.Services.CurriculumAPI.Models.DTO
{
    public class WeekUpdateDto
    {
        [Required]
        public string WeekCode { get; set; }

        [Required]
        [Range(1, 52)]
        public int WeekNumber { get; set; }

        // CourseCode is included but not used in updates
        public string CourseCode { get; set; }
    }
}
