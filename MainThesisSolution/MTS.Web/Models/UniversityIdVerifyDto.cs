namespace MTS.Web.Models
{
    public class UniversityIdVerifyDto
    {
        public string Code { get; set; }
        public string Type { get; set; } // "STUDENT" or "PROFESSOR"
    }
}
