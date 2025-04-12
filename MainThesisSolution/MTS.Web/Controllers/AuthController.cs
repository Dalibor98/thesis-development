using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MTS.Web.Service.IService;
using MTS.Web.Models;
using MTS.Web.Utility;
using MTS.Web.Models.User.UniId;
using MTS.Web.Models.User.Student;
using MTS.Web.Models.User.Professor;
using MTS.Web.Models.Auth;
namespace MTS.Web.Controllers

{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;
        private readonly IStudentService _studentService;
        private readonly IProfessorService _professorService;
        private readonly IUniversityIdService _universityIdService;

        public AuthController(IAuthService authService, ITokenProvider tokenProvider, IProfessorService professorService,IStudentService studentService,IUniversityIdService universityIdService)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
            _professorService = professorService;
            _studentService = studentService;
            _universityIdService = universityIdService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = new();
            return View(loginRequestDto);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto obj)
        {
            ResponseDto responseDto = await _authService.LoginAsync(obj);

            if (responseDto != null && responseDto.IsSuccess)
            {
                LoginResponseDto loginResponseDto =
                    JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));

                await SignInUser(loginResponseDto);
                _tokenProvider.SetToken(loginResponseDto.Token);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = responseDto.Message;
                return View(obj);
            }
        }    

        public IActionResult Register()
        {
            return View();
        }

        // Student Registration GET method
        public async Task<IActionResult> RegisterStudent()
        {
            // Fetch an unassigned student university ID
            List<UniversityIdentifierDto> list = new List<UniversityIdentifierDto>();
            ResponseDto unassignedIdsResponse = await _universityIdService.GetUnassignedIdsAsync("STUDENT");
            if (unassignedIdsResponse != null && unassignedIdsResponse.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<UniversityIdentifierDto>>(Convert.ToString(unassignedIdsResponse.Result));
                
                
                // Prepare the view model with the first unassigned ID
                var studentCreateDto = new StudentCreateDto
                {
                    UniversityId = list?.FirstOrDefault()?.Code
                };

                return View(studentCreateDto);
            }
            return RedirectToAction(nameof(Register));
        
        }
        [HttpPost]
        public async Task<IActionResult> RegisterStudent(StudentCreateDto studentDto)
        {
            if (!ModelState.IsValid)
            {
                return View(studentDto);
            }

            // Step 1: Create Auth User first
            var registrationRequest = new RegistrationRequestDto
            {
                Email = studentDto.Email,
                Name = studentDto.Name,
                UniversityId = studentDto.UniversityId,
                Password = studentDto.Password,
                Role = SD.RoleSidekick,
            };

            ResponseDto? userCreated = await _authService.RegisterAsync(registrationRequest);

            if (userCreated == null || !userCreated.IsSuccess)
            {
                TempData["error"] = "User registration failed: " + (userCreated?.Message ?? "Unknown error");
                return View(studentDto);
            }

            // Step 2: Create Student record only if Auth User succeeds
            var result = await _studentService.CreateStudentAsync(studentDto);

            if (result == null || !result.IsSuccess)
            {
                // Attempt to delete the auth user to maintain consistency
                await _authService.DeleteAsync(studentDto.Email);
                TempData["error"] = "Student creation failed: " + (result?.Message ?? "Unknown error");
                return View(studentDto);
            }

            TempData["success"] = "Student registered successfully";
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> RegisterProfessor()
        {
            // Fetch an unassigned professor university ID
            List<UniversityIdentifierDto> list = new List<UniversityIdentifierDto>();
            ResponseDto unassignedIdsResponse = await _universityIdService.GetUnassignedIdsAsync("PROFESSOR");
            if (unassignedIdsResponse != null && unassignedIdsResponse.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<UniversityIdentifierDto>>(Convert.ToString(unassignedIdsResponse.Result));

                // Prepare the view model with the first unassigned ID
                var professorCreateDto = new ProfessorCreateDto
                {
                    UniversityId = list?.FirstOrDefault()?.Code
                };
                return View(professorCreateDto);
            }
            return RedirectToAction(nameof(Register));
        }
        [HttpPost]
        public async Task<IActionResult> RegisterProfessor(ProfessorCreateDto professorDto)
        {
            if (!ModelState.IsValid)
            {
                return View(professorDto);
            }

            //1. Create Auth User first
            var registrationRequest = new RegistrationRequestDto
            {
                Email = professorDto.Email,
                Name = professorDto.Name,
                UniversityId = professorDto.UniversityId,
                Password = professorDto.Password,
                Role = SD.RoleLeader,
            };

            ResponseDto? userCreated = await _authService.RegisterAsync(registrationRequest);

            if (userCreated == null || !userCreated.IsSuccess)
            {
                TempData["error"] = "User registration failed: " + (userCreated?.Message ?? "Unknown error");
                return View(professorDto);
            }
            //2.Create professor only if user is created
            var result = await _professorService.CreateProfessorAsync(professorDto);

            if (result == null || !result.IsSuccess)
            {
                //if there was any error, delete previously created user.
                await _authService.DeleteAsync(professorDto.Email);
                TempData["error"] = "Professor creation failed: " + (result?.Message ?? "Unknown error");

                return View(professorDto);
            }

            TempData["success"] = "Professor registered successfully";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();

            return RedirectToAction("Index", "Home");
        }
        private async Task SignInUser(LoginResponseDto model)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(model.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));
            identity.AddClaim(new Claim("UniversityId", model.User.UniversityId));

            identity.AddClaim(new Claim(ClaimTypes.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role,
                jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));
            

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

    }
}
