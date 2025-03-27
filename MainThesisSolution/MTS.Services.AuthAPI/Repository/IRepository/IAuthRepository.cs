using MTS.Services.AuthAPI.Models.DTO;

namespace MTS.Services.AuthAPI.Repository.IRepository
{
    public interface IAuthRepository
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<bool> AssignRole(string email, string roleName);
        Task<ResponseDto> DeleteUser(string email);
    }
}
