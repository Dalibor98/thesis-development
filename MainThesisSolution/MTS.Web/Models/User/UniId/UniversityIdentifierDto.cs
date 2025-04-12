using static MTS.Web.Utility.SD;

namespace MTS.Web.Models.User.UniId
{
    public class UniversityIdentifierDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public IdType Type { get; set; } // "STUDENT" or "PROFESSOR"
        public bool IsAssigned { get; set; }
    }
}
