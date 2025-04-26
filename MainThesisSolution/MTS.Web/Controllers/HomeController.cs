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
using MTS.Web.Service.IService;

namespace MTS.Web.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IQuizService _quizService;
        private readonly IStudentQuizAttemptService _studentQuizAttemptService;

        public HomeController(
            ICourseService courseService,
            IQuizService quizService,
            IStudentQuizAttemptService studentQuizAttemptService)
        {
            _courseService = courseService;
            _quizService = quizService;
            _studentQuizAttemptService = studentQuizAttemptService;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole(SD.RoleAdmin))
                {
                    // Admin landing page
                    return RedirectToAction("Admin", "Admin");
                }
                else if (User.IsInRole(SD.RoleLeader))
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
        [Authorize]
        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> ProfessorDashboard()
        {
            var professorId = User.FindFirstValue("UniversityId");

            // Get professor's courses
            var coursesResponse = await _courseService.GetProfessorCoursesAsync(professorId);
            if (coursesResponse != null && coursesResponse.IsSuccess)
            {
                ViewBag.Courses = JsonConvert.DeserializeObject<List<CourseDto>>(Convert.ToString(coursesResponse.Result));
            }
            else
            {
                ViewBag.Courses = new List<CourseDto>();
            }

            // Get recent quiz attempts
            var quizAttemptsResponse = await _studentQuizAttemptService.GetRecentAttemptsByProfessorIdAsync(professorId);
            if (quizAttemptsResponse != null && quizAttemptsResponse.IsSuccess)
            {
                ViewBag.QuizAttempts = JsonConvert.DeserializeObject<List<StudentQuizAttemptDto>>(Convert.ToString(quizAttemptsResponse.Result));
            }
            else
            {
                ViewBag.QuizAttempts = new List<StudentQuizAttemptDto>();
            }

            // Get text-based quizzes that need grading
            var textBasedQuizzesResponse = await _quizService.GetTextBasedQuizzesWithPendingGradingAsync(professorId);
            if (textBasedQuizzesResponse != null && textBasedQuizzesResponse.IsSuccess)
            {
                ViewBag.TextBasedQuizzes = JsonConvert.DeserializeObject<List<QuizWithAttemptsViewModel>>(Convert.ToString(textBasedQuizzesResponse.Result));
            }
            else
            {
                ViewBag.TextBasedQuizzes = new List<QuizWithAttemptsViewModel>();
            }

            return View();
        }
        [Authorize]
        [Authorize(Roles = SD.RoleSidekick)]
        public async Task<IActionResult> StudentDashboard()
        {
            var studentId = User.FindFirstValue("UniversityId");

            // Get student's courses
            var coursesResponse = await _courseService.GetStudentCoursesAsync(studentId);
            if (coursesResponse != null && coursesResponse.IsSuccess)
            {
                ViewBag.Courses = JsonConvert.DeserializeObject<List<CourseDto>>(Convert.ToString(coursesResponse.Result));
            }
            else
            {
                ViewBag.Courses = new List<CourseDto>();
            }

            // Get upcoming quizzes
            var quizzesResponse = await _quizService.GetUpcomingQuizzesByStudentIdAsync(studentId);
            if (quizzesResponse != null && quizzesResponse.IsSuccess)
            {
                ViewBag.Quizzes = JsonConvert.DeserializeObject<List<QuizDto>>(Convert.ToString(quizzesResponse.Result));
            }
            else
            {
                ViewBag.Quizzes = new List<QuizDto>();
            }
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