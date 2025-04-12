using MTS.Web.Models.User;

namespace MTS.Web.Models.Auth
{
    public class LoginResponseDto
    {
        public UserDto User { get; set; }
        public string Token { get; set; }
    }
}
