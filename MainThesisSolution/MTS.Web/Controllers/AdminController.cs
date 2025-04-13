 using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MTS.Web.Models;
using MTS.Web.Models.User.Professor;
using MTS.Web.Models.User.Student;
using MTS.Web.Models.User.UniId;
using MTS.Web.Service;
using MTS.Web.Service.IService;
using Newtonsoft.Json;
using System.Reflection;

namespace MTS.Web.Controllers
{
    public class AdminController : Controller
    {

        private readonly IAdminService _adminService;
        private readonly IProfessorService _professorService;
        private readonly IAuthService _authService;
       

        public AdminController(IAdminService adminService, IProfessorService professorService,IAuthService authService)
        {
            _adminService = adminService;
            _professorService = professorService;
            _authService = authService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult UniversityIdGenerate()
        {
            UniversityIdGenerateDto universityIdGenerateDto = new();
            return View(universityIdGenerateDto);

        }

        [HttpPost]
        public async Task<IActionResult> UniversityIdGenerate(UniversityIdGenerateDto obj)
        {
            ResponseDto responseDto = await _adminService.GenerateIds(obj);

            if (responseDto != null && responseDto.IsSuccess)
            {
                var generatedIds = JsonConvert.DeserializeObject<List<string>>(responseDto.Result.ToString());
                return View("GeneratedIds", generatedIds);
            }
            else
            {
                TempData["error"] = responseDto.Message;
                return View(obj);
            }
        }

        [HttpGet]
        public IActionResult Admin()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> StudentIndex()
        {
            List<StudentDto>? list = new();
            ResponseDto? response = await _adminService.GetStudentsAsync();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<StudentDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(list);
        }


        public async Task<IActionResult> StudentEdit(int studentId)
        {
            ResponseDto? response = await _adminService.GetStudentByIdAsync(studentId);
            if (response != null && response.IsSuccess)
            {
                StudentDto? model = JsonConvert.DeserializeObject<StudentDto>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> StudentEdit(StudentDto studentDto)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _adminService.UpdateStudentAsync(studentDto);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product updated successfully";
                    return RedirectToAction(nameof(StudentIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(studentDto);
        }

        [HttpGet]
        public async Task<IActionResult> ProfessorIndex()
        {
            List<ProfessorDto>? list = new();
            ResponseDto? response = await _adminService.GetProfessorsAsync();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProfessorDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(list);
        }
        
        public async Task<IActionResult> StudentDelete(int studentId)
        {
            ResponseDto? response = await _adminService.GetStudentByIdAsync(studentId);

            if (response != null && response.IsSuccess)
            {
                StudentDto? model = JsonConvert.DeserializeObject<StudentDto>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }

        [HttpPost]
        [ActionName("StudentDelete")]
        public async Task<IActionResult> StudentDeleteConfirmed(int studentId,string email)
        {
            ResponseDto? userDeleted = await _authService.DeleteAsync(email);

            if (userDeleted != null && userDeleted.IsSuccess)
            {
                ResponseDto? studentDeleted = await _adminService.DeleteStudentAsync(studentId);
                if (studentDeleted != null && studentDeleted.IsSuccess)
                {
                    TempData["success"] = "Student deleted successfully";
                    return RedirectToAction(nameof(StudentIndex));
                }
                TempData["fail"] = studentDeleted?.Message;
                return RedirectToAction(nameof(StudentIndex));
            }
            else
            {
                TempData["error"] = userDeleted?.Message;
            }
            return RedirectToAction(nameof(StudentIndex));
        }

        public async Task<IActionResult> ProfessorEdit(int professorId)
        {
            ResponseDto? response = await _professorService.GetProfessorByIdAsync(professorId);
            if (response != null && response.IsSuccess)
            {
                ProfessorDto? model = JsonConvert.DeserializeObject<ProfessorDto>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ProfessorEdit(ProfessorDto professorDto)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _professorService.UpdateProfessorAsync(professorDto);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product updated successfully";
                    return RedirectToAction(nameof(ProfessorIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(professorDto);
        }

        public async Task<IActionResult> ProfessorDelete(int professorId)
        {
            ResponseDto? response = await _professorService.GetProfessorByIdAsync(professorId);

            if (response != null && response.IsSuccess)
            {
                ProfessorDto? model = JsonConvert.DeserializeObject<ProfessorDto>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }

        [HttpPost]
        [ActionName("ProfessorDelete")]
        public async Task<IActionResult> ProfessortDeleteConfirmed(int professorId)
        {
            ResponseDto? response = await _professorService.DeleteProfessorAsync(professorId);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Student deleted successfully";
                return RedirectToAction(nameof(ProfessorIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return RedirectToAction(nameof(ProfessorIndex));
        }

        /*
        [HttpPost]
        public async Task<IActionResult> VerifyUniversityId(UniversityIdVerifyDto verifyDto)
        {
            ResponseDto? response = await _adminService.VerifyId(verifyDto);
            if (response != null && response.IsSuccess)
            {
                // Handle success case
                bool isValid = JsonConvert.DeserializeObject<bool>(Convert.ToString(response.Result));
                if (isValid)
                {
                    TempData["success"] = "University ID verified successfully";
                }
                else
                {
                    TempData["error"] = "Invalid University ID";
                }
            }
            else
            {
                TempData["error"] = response?.Message;
                return RedirectToAction(nameof(Register));
            }
        }
        */
        /*
        public async Task<IActionResult> ProfessorDelete(int professorId)
        {
            ResponseDto? response = await _adminService.GetProfessorByIdAsync(professorId);

            if (response != null && response.IsSuccess)
            {
                ProfessorDto? model = JsonConvert.DeserializeObject<ProfessorDto>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }
        */
        /*
        [HttpPost]
        public async Task<IActionResult> ProfessorDelete(ProfessorDto professorDto)
        {
            ResponseDto? response = await _adminService.DeleteProfessorAsync(professorDto.ID);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Professor deleted successfully";
                return RedirectToAction(nameof(ProfessorIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(professorDto);
        }
        */
    }
}
