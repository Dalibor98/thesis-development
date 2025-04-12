using MTS.Web.Models.User;

namespace MTS.Web.Models.User.Professor
{
    public class ProfessorDto : UserDto
    {
        public string Department { get; set; }
        public string Title { get; set; }
    }
}
