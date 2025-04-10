namespace MTS.Services.CurriculumAPI.Models.DTO
{
    public class AssignmentCreateDto
    {
        public string CourseCode { get; set; }
        public string WeekCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int MaxPoints { get; set; }
        public int MinPoints { get; set; }
        public DateTime DueDate { get; set; }
    }
}
