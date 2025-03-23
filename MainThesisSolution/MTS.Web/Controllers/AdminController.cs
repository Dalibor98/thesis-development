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

        

        /*
        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDto>? list = new();

            ResponseDto? response = await _couponService.GetAllCouponsAsync();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(list);
        }
        */

    }
}
