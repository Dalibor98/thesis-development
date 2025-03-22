namespace MTS.Services.UserAPI.Models
{
    public class Professor : User
    {
        public string Department { get; set; }
        public string Title { get; set; } // "Assistant Professor", "Associate Professor", etc.
    }
}
