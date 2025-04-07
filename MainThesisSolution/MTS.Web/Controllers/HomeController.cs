using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MTS.Web.Models;
using MTS.Web.Utility;

namespace MTS.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole(SD.RoleLeader))
                {
                    // Professor landing page
                    return View("ProfessorDashboard");
                }
                else if (User.IsInRole(SD.RoleSidekick))
                {
                    // Student landing page
                    return View("StudentDashboard");
                }
            }

            // Default landing page for non-authenticated users
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
