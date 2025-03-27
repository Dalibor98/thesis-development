namespace MTS.Services.AuthAPI.Models.DTO
{
    public class RegistrationRequestDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string UniversityId { get; set; }
        public string? Role { get; set; }
    }
}
