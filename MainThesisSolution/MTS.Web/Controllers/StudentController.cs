using Microsoft.AspNetCore.Mvc;

namespace MTS.Web.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
