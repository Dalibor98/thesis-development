namespace MTS.Web.Models
{
    public class UniversityIdGenerateDto
    {
        public string Type { get; set; } // "STUDENT" or "PROFESSOR"
        public int Count { get; set; }
    }
}
