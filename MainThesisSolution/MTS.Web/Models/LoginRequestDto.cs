using System.ComponentModel.DataAnnotations;

namespace MTS.Web.Models
{
    public class LoginRequestDto
    {
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
