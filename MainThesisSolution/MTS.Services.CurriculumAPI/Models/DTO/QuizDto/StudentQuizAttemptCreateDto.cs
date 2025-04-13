namespace MTS.Services.CurriculumAPI.Models.DTO.QuizDto
{
    public class StudentQuizAttemptCreateDto
    {
        public string QuizCode { get; set; }
        public string StudentUniversityId { get; set; }
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; }
        public int Score { get; set; } = 0;
    }
}
