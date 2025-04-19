using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTS.Web.Models.Curriculum.Assignment;
using MTS.Web.Models.Curriculum.Course;
using MTS.Web.Models.Curriculum.Material;
using MTS.Web.Models.Curriculum.Quiz;
using MTS.Web.Models.Curriculum.Week;
using MTS.Web.Service.IService;
using MTS.Web.Utility;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MTS.Web.Controllers
{
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IEnrollmentService _enrollmentService;

        public CourseController(ICourseService courseService, IEnrollmentService enrollmentService)
        {
            _courseService = courseService;
            _enrollmentService = enrollmentService;
        }

        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> ProfessorCourses()
        {
            var professorId = User.FindFirstValue("UniversityId");
            var response = await _courseService.GetProfessorCoursesAsync(professorId);

            List<CourseDto> list = new();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<CourseDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(list);
        }

        public async Task<IActionResult> Details(string courseCode)
        {
            var response = await _courseService.GetCourseByCodeAsync(courseCode);

            if (response != null && response.IsSuccess)
            {
                CourseDto course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(response.Result));

                // Get weeks
                var weeksResponse = await _courseService.GetWeeksByCourseCodeAsync(courseCode);
                if (weeksResponse != null && weeksResponse.IsSuccess)
                {
                    ViewBag.Weeks = JsonConvert.DeserializeObject<List<WeekDto>>(Convert.ToString(weeksResponse.Result));
                }
                else
                {
                    ViewBag.Weeks = new List<WeekDto>();
                }

                // Get materials
                var materialsResponse = await _courseService.GetMaterialsByCourseCodeAsync(courseCode);
                if (materialsResponse != null && materialsResponse.IsSuccess)
                {
                    ViewBag.Materials = JsonConvert.DeserializeObject<List<MaterialDto>>(Convert.ToString(materialsResponse.Result));
                }
                else
                {
                    ViewBag.Materials = new List<MaterialDto>();
                }

                // Get assignments
                var assignmentsResponse = await _courseService.GetAssignmentsByCourseCodeAsync(courseCode);
                if (assignmentsResponse != null && assignmentsResponse.IsSuccess)
                {
                    ViewBag.Assignments = JsonConvert.DeserializeObject<List<AssignmentDto>>(Convert.ToString(assignmentsResponse.Result));
                }
                else
                {
                    ViewBag.Assignments = new List<AssignmentDto>();
                }

                // Get quizzes
                var quizzesResponse = await _courseService.GetQuizzesByCourseCodeAsync(courseCode);
                if (quizzesResponse != null && quizzesResponse.IsSuccess)
                {
                    ViewBag.Quizzes = JsonConvert.DeserializeObject<List<QuizDto>>(Convert.ToString(quizzesResponse.Result));
                }
                else
                {
                    ViewBag.Quizzes = new List<QuizDto>();
                }

                // Check if student is enrolled in this course
                if (User.IsInRole(SD.RoleSidekick))
                {
                    var studentId = User.FindFirstValue("UniversityId");
                    var enrollmentResponse = await _enrollmentService.IsStudentEnrolledAsync(courseCode, studentId);

                    if (enrollmentResponse != null && enrollmentResponse.IsSuccess)
                    {
                        bool isEnrolled = Convert.ToBoolean(enrollmentResponse.Result);
                        ViewBag.IsEnrolled = isEnrolled;
                    }
                    else
                    {
                        ViewBag.IsEnrolled = false;
                    }
                }
                else
                {
                    ViewBag.IsEnrolled = false;
                }

                return View(course);
            }

            TempData["error"] = response?.Message;
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = SD.RoleLeader)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> Create(CourseCreateDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _courseService.CreateCourseAsync(model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Course created successfully";
                    return RedirectToAction(nameof(ProfessorCourses));
                }
                else
                {
                    if (response?.Message?.Contains("already exists") == true)
                    {
                        TempData["error"] = response.Message;
                        return View(model);
                    }

                    TempData["error"] = response?.Message ?? "An error occurred while creating the course";
                }
            }
            return View(model);
        }

        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> Edit(string courseCode)
        {
            var response = await _courseService.GetCourseByCodeAsync(courseCode);

            if (response != null && response.IsSuccess)
            {
                CourseDto course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(response.Result));

                // Verify that the current user is the professor for this course
                var professorId = User.FindFirstValue("UniversityId");
                if (course.ProfessorUniversityId != professorId)
                {
                    TempData["error"] = "You are not authorized to edit this course";
                    return RedirectToAction(nameof(ProfessorCourses));
                }

                CourseUpdateDto courseUpdateDto = new ()
                {
                    CourseCode = course.CourseCode,
                    Title = course.Title,
                    Description = course.Description,
                    ProfessorUniversityId = course.ProfessorUniversityId
                };

                return View(courseUpdateDto);
            }

            TempData["error"] = response?.Message;
            return RedirectToAction(nameof(ProfessorCourses));
        }

        [HttpPost]
        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> Edit(CourseUpdateDto model)
        {
            if (ModelState.IsValid)
            {
                var professorId = User.FindFirstValue("UniversityId");

                // Ensure the logged-in professor is the course owner
                if (model.ProfessorUniversityId != professorId)
                {
                    TempData["error"] = "You are not authorized to edit this course";
                    return RedirectToAction(nameof(ProfessorCourses));
                }

                var courseUpdateDto = new CourseUpdateDto
                {
                    CourseCode = model.CourseCode,
                    Title = model.Title,
                    Description = model.Description,
                    ProfessorUniversityId = model.ProfessorUniversityId
                };

                var response = await _courseService.UpdateCourseAsync(courseUpdateDto);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Course updated successfully";
                    return RedirectToAction(nameof(ProfessorCourses));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> Delete(int id)
        {
            // First get the course to verify ownership
            var course = await _courseService.GetCourseByIdAsync(id);

            if (course != null && course.IsSuccess)
            {
                var courseDto = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(course.Result));

                // Verify ownership
                var professorId = User.FindFirstValue("UniversityId");
                if (courseDto.ProfessorUniversityId != professorId)
                {
                    TempData["error"] = "You are not authorized to delete this course";
                    return RedirectToAction(nameof(ProfessorCourses));
                }

                var response = await _courseService.DeleteCourseAsync(id);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Course deleted successfully";
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            else
            {
                TempData["error"] = "Course not found";
            }

            return RedirectToAction(nameof(ProfessorCourses));
        }

        // Action for all courses (course catalog)
        [Authorize(Roles = SD.RoleSidekick)]
        public async Task<IActionResult> Index()
        {
            var response = await _courseService.GetAllCoursesAsync();

            List<CourseDto> list = new();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<CourseDto>>(Convert.ToString(response.Result));
            }

            return View(list);
        }
    }
}