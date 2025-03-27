using System.ComponentModel.DataAnnotations;

namespace MTS.Web.Models
{
    public class RegistrationRequestDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string UniversityId { get; set; }
        public string Role { get; set; }
    }
}
