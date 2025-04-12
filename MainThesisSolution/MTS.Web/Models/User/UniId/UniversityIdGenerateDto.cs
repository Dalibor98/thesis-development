namespace MTS.Web.Models.User.UniId
{
    public class UniversityIdGenerateDto
    {
        public string Type { get; set; } // "STUDENT" or "PROFESSOR"
        public int Count { get; set; }
    }
}
