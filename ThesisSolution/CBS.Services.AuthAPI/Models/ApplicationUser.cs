using Microsoft.AspNetCore.Identity;

namespace CBS.Services.AuthAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
