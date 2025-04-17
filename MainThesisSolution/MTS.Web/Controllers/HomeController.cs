using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MTS.Web.Models;
using MTS.Web.Models.Curriculum.Assignment;
using MTS.Web.Models.Curriculum.Course;
using MTS.Web.Models.Curriculum.Quiz;
using MTS.Web.Utility;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using MTS.Web.Service.IService;

namespace MTS.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IQuizService _quizService;
        private readonly IAssignmentService _assignmentService;
        public HomeController(ICourseService courseService,IQuizService quizService,IAssignmentService assignmentService)
        {
            _courseService = courseService;
            _quizService = quizService;
            _assignmentService = assignmentService;
        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole(SD.RoleLeader))
                {
                    // Professor landing page
                    return RedirectToAction("ProfessorDashboard");
                }
                else if (User.IsInRole(SD.RoleSidekick))
                {
                    // Student landing page
                    return RedirectToAction("StudentDashboard");
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
