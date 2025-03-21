namespace MTS.Services.UserAPI.Models.DTO
{
    public class UniversityIdVerifyDto
    {
        public string Code { get; set; }
        public string Type { get; set; } // "STUDENT" or "PROFESSOR"
    }
}
