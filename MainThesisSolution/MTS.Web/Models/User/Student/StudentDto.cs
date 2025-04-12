using MTS.Web.Models.User;

namespace MTS.Web.Models.User.Student
{
    public class StudentDto : UserDto
    {
        public string Major { get; set; }
        public int EnrollmentYear { get; set; }
    }
}
