namespace MTS.Services.UserAPI.Models
{
    public class Student : User
    {
        public string Major { get; set; }
        public int EnrollmentYear { get; set; }
    }
}
