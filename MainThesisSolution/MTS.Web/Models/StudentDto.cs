namespace MTS.Web.Models
{
    public class StudentDto : UserDto
    {
        public string Major { get; set; }
        public int EnrollmentYear { get; set; }
    }
}
