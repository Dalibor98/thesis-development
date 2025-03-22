namespace MTS.Services.UserAPI.Models.DTO
{
    public class UniversityIdGenerateDto
    {
        public string Type { get; set; } // "STUDENT" or "PROFESSOR"
        public int Count { get; set; }
    }
}
