using CBS.Services.AuthAPI.Data;
using CBS.Services.AuthAPI.Models;
using CBS.Services.AuthAPI.Models.DTO;
using CBS.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;

namespace CBS.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AppDbContext db, IJwtTokenGenerator jwtTokenGenerator,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<bool> AssignRole(string email, string roleName)
        {
            //check whether the user with this email already exists
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            //if the user with that email is found 
            if (user != null)
            {   //check whether he aready has this role
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    //create role if it does not exist
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                } //add that role to the user.
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;

        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {   //check whether user with email sent from loginRequestDto exists already - user of null
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());
            //immediately in case it exists whether there is a password associated with the user and whether its correct - true or false
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            //here both user and password are checked
            if (user == null || isValid == false)
            {   //empty response is given back in case something is faulty
                return new LoginResponseDto() { User = null, Token = "" };
            }
            //if the user exist and password is alright we get its roles
            var roles = await _userManager.GetRolesAsync(user);
            //we create token based on his roles
            var token = _jwtTokenGenerator.GenerateToken(user, roles);
            //UserDto creation
             UserDto userDTO = new()
            {
                Email = user.Email,
                ID = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };
            //login response is created in which we assign token with the user and return it back - Question for Jay whether this was neccesarry
            //why not create loginResponseDto only and just fill every property inside it?
            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User = userDTO,
                Token = token
            };

            return loginResponseDto;
        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            //create app user with details from registrationRequest.
            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                PhoneNumber = registrationRequestDto.PhoneNumber
            };

            try
            {   //clarify this part here,main concern:user object is already created without password, but now we use userManager to "create" a object
                //of IdentityResult type and we do that by combining user object with password,desc: creates a user with given password in the *BACKING STORE?*
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);


                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers.First(u => u.UserName == registrationRequestDto.Email);

                    UserDto userDto = new()
                    {
                        Email = userToReturn.Email,
                        ID = userToReturn.Id,
                        Name = userToReturn.Name,
                        PhoneNumber = userToReturn.PhoneNumber
                    };

                    return "";

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
    }
}
