using Microsoft.AspNetCore.Identity;

namespace MTS.Services.AuthAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string UniversityId { get; set; }
    }
}
