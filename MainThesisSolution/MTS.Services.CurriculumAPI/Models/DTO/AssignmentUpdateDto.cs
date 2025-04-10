using System.ComponentModel.DataAnnotations;

namespace MTS.Services.CurriculumAPI.Models.DTO
{
    public class AssignmentUpdateDto
    {
        public string CourseCode { get; set; }
        public string WeekCode { get; set; }
        public string AssignmentCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int MaxPoints { get; set; } = 100;
        public int MinPoints { get; set; } = 60;
        public DateTime DueDate { get; set; }
    }
}
