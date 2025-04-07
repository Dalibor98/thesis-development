using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MTS.Services.AuthAPI.Data;
using MTS.Services.AuthAPI.Models;
using MTS.Services.AuthAPI.Models.DTO;
using MTS.Services.AuthAPI.Repository.IRepository;
using MTS.Services.AuthAPI.Service.IService;

namespace MTS.Services.AuthAPI.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AuthDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthRepository(AuthDbContext db, IJwtTokenGenerator jwtTokenGenerator,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(roleName))
            {
                return false;
            }

            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (user == null)
            {
                return false;
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                //create role if it does not exist
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (!roleResult.Succeeded)
                {
                    return false;
                }
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            return result.Succeeded;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

            if (user == null || isValid == false)
            {
                return new LoginResponseDto() { User = null, Token = "" };
            }

            //if user was found , Generate JWT Token
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            UserDto userDTO = new()
            {
                Email = user.Email,
                ID = user.Id,
                Name = user.Name,
                UniversityId = user.UniversityId
            };

            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User = userDTO,
                Token = token
            };

            return loginResponseDto;
        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                UniversityId = registrationRequestDto.UniversityId
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);

                if (result.Succeeded)
                {   
                    var userToReturn = _db.ApplicationUsers.First(u => u.UserName == registrationRequestDto.Email);

                    //pay attention for final code, we dont need userDto here ! 
                    var addRole = await AssignRole(userToReturn.Email, registrationRequestDto.Role);
                    if (addRole)
                    {
                        return "";
                    }
                    /*
                    UserDto userDto = new()
                    {
                        Email = userToReturn.Email,
                        ID = userToReturn.Id, 
                        Name = userToReturn.Name,
                    };
                    */

                    return "Error while adding a role";

                   
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }

            }
            catch (Exception ex)
            {

            }
            return "Error Encountered";
        }

        public async Task<ResponseDto> DeleteUser(string email)
        {
            var response = new ResponseDto();

            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    response.IsSuccess = false;
                    response.Message = "User not found.";
                    return response;
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, userRoles);

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    response.IsSuccess = true;
                    response.Message = "User deleted successfully.";
                    response.Result = new UserDto
                    {
                        ID = user.Id,
                        Email = user.Email,
                        Name = user.Name
                    };
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Failed to delete user.";
                    response.Result = result.Errors;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred while deleting the user.";
                response.Result = ex.Message;
            }
            return response;
        }
    }
}
