using Microsoft.AspNetCore.Mvc;
using MTS.Web.Models;
using MTS.Web.Service.IService;
using Newtonsoft.Json;

namespace MTS.Web.Controllers
{
    public class AdminController : Controller
    {

        private readonly IAdminService _adminService;
        

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
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

    }
}
