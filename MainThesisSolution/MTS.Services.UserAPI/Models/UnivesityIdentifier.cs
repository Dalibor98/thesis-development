using MTS.Services.UserAPI.Utility;

namespace MTS.Services.UserAPI.Models
{
    public class UniversityIdentifier
    {
        public int Id { get; set; }
        public string Code { get; set; } 
        public IdType Type { get; set; } // "STUDENT" or "PROFESSOR"
        public bool IsAssigned { get; set; } 
    }
}
