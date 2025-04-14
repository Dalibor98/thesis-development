using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTS.Web.Models.Curriculum.Assignment;
using MTS.Web.Models.Curriculum.Course;
using MTS.Web.Models.Curriculum.Material;
using MTS.Web.Models.Curriculum.Quiz;
using MTS.Web.Models.Curriculum.Week;
using MTS.Web.Service;
using MTS.Web.Service.IService;
using MTS.Web.Utility;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MTS.Web.Controllers
{
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IQuizService _quizService;
        private readonly IEnrollmentService _enrollmentService;

        public CourseController(ICourseService courseService, IQuizService quizService, IEnrollmentService enrollmentService)
        {
            _courseService = courseService;
            _quizService = quizService;
            _enrollmentService = enrollmentService;
        }

        public async Task<IActionResult> Index()
        {
            List<CourseDto> courses = new();

            var response = await _courseService.GetAllCoursesAsync();
            if (response != null && response.IsSuccess)
            {
                courses = JsonConvert.DeserializeObject<List<CourseDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(courses);
        }

        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> ProfessorCourses()
        {
            List<CourseDto> courses = new();

            // Get current user's university ID
            var userUniversityId = User.FindFirstValue(claimType: "UniversityId");
            if (string.IsNullOrEmpty(userUniversityId))
            {
                TempData["error"] = "Unable to determine your university ID";
                return RedirectToAction(nameof(Index));
            }

            var response = await _courseService.GetProfessorCoursesAsync(userUniversityId);
            if (response != null && response.IsSuccess)
            {
                courses = JsonConvert.DeserializeObject<List<CourseDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(courses);
        }

        [Authorize(Roles = SD.RoleLeader)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> Create(TemporaryCourseDTO courseDto)
        {
            var userUniversityId = User.FindFirstValue("UniversityId");
            courseDto.ProfessorUniversityId = userUniversityId;

            ModelState.Remove("ProfessorUniversityId");


            if (ModelState.IsValid)
            {
                var response = await _courseService.CreateCourseAsync(courseDto);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Course created successfully";
                    return RedirectToAction(nameof(ProfessorCourses));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }

            return View(courseDto);
        }

        public async Task<IActionResult> View(string quizCode)
        {
            var response = await _quizService.GetQuizByCodeAsync(quizCode);

            if (response != null && response.IsSuccess)
            {
                var quiz = JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(response.Result));

                // For professors, load questions and answers
                if (User.IsInRole(SD.RoleLeader))
                {
                    var questionsResponse = await _quizService.GetQuestionsByQuizCodeAsync(quizCode);
                    if (questionsResponse != null && questionsResponse.IsSuccess)
                    {
                        var questions = JsonConvert.DeserializeObject<List<QuizQuestionDto>>(
                            Convert.ToString(questionsResponse.Result));
                        ViewBag.Questions = questions;

                        // Get answers for each question
                        var questionAnswers = new Dictionary<string, List<AnswerDto>>();
                        foreach (var question in questions)
                        {
                            var answersResponse = await _quizService.GetAnswersForQuestionAsync(question.QuizQuestionCode);
                            if (answersResponse != null && answersResponse.IsSuccess)
                            {
                                var answers = JsonConvert.DeserializeObject<List<AnswerDto>>(
                                    Convert.ToString(answersResponse.Result));
                                questionAnswers[question.QuizQuestionCode] = answers;
                            }
                            else
                            {
                                questionAnswers[question.QuizQuestionCode] = new List<AnswerDto>();
                            }
                        }

                        ViewBag.QuestionAnswers = questionAnswers;
                    }
                    else
                    {
                        ViewBag.Questions = new List<QuizQuestionDto>();
                        ViewBag.QuestionAnswers = new Dictionary<string, List<AnswerDto>>();
                    }

                    // Get a course to check ownership for edit permissions
                    var courseResponse = await _courseService.GetCourseByCodeAsync(quiz.CourseCode);
                    if (courseResponse != null && courseResponse.IsSuccess)
                    {
                        ViewBag.Course = JsonConvert.DeserializeObject<CourseDto>(
                            Convert.ToString(courseResponse.Result));
                    }
                }

                // For students, check if they have any attempts
                if (User.IsInRole(SD.RoleSidekick))
                {
                    var studentId = User.FindFirstValue("UniversityId");
                    var attemptsResponse = await _quizService.GetAttemptsByStudentIdAsync(studentId);

                    if (attemptsResponse != null && attemptsResponse.IsSuccess)
                    {
                        var attempts = JsonConvert.DeserializeObject<List<StudentQuizAttemptDto>>(
                            Convert.ToString(attemptsResponse.Result));

                        ViewBag.Attempts = attempts.Where(a => a.QuizCode == quizCode).ToList();
                    }
                    else
                    {
                        ViewBag.Attempts = new List<StudentQuizAttemptDto>();
                    }
                }

                return View(quiz);
            }

            TempData["error"] = response?.Message ?? "Quiz not found";
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Details(string courseCode)
        {
            var response = await _courseService.GetCourseByCodeAsync(courseCode);

            if (response != null && response.IsSuccess)
            {
                var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(response.Result));

                // Get weeks for this course
                var weeksResponse = await _courseService.GetWeeksByCourseCodeAsync(courseCode);
                if (weeksResponse != null && weeksResponse.IsSuccess)
                {
                    ViewBag.Weeks = JsonConvert.DeserializeObject<List<WeekDto>>(Convert.ToString(weeksResponse.Result));
                }
                else
                {
                    ViewBag.Weeks = new List<WeekDto>();
                }

                // Get materials for this course
                var materialsResponse = await _courseService.GetMaterialsByCourseCodeAsync(courseCode);
                if (materialsResponse != null && materialsResponse.IsSuccess)
                {
                    ViewBag.Materials = JsonConvert.DeserializeObject<List<MaterialDto>>(Convert.ToString(materialsResponse.Result));
                }
                else
                {
                    ViewBag.Materials = new List<MaterialDto>();
                }

                // Get assignments for this course
                var assignmentsResponse = await _courseService.GetAssignmentsByCourseCodeAsync(courseCode);
                if (assignmentsResponse != null && assignmentsResponse.IsSuccess)
                {
                    ViewBag.Assignments = JsonConvert.DeserializeObject<List<AssignmentDto>>(Convert.ToString(assignmentsResponse.Result));
                }
                else
                {
                    ViewBag.Assignments = new List<AssignmentDto>();
                }

                // Get quizzes for this course
                var quizzesResponse = await _courseService.GetQuizzesByCourseCodeAsync(courseCode);
                if (quizzesResponse != null && quizzesResponse.IsSuccess)
                {
                    ViewBag.Quizzes = JsonConvert.DeserializeObject<List<QuizDto>>(Convert.ToString(quizzesResponse.Result));
                }
                else
                {
                    ViewBag.Quizzes = new List<QuizDto>();
                }

                // Check if the student is already enrolled
                if (User.Identity.IsAuthenticated && User.IsInRole(SD.RoleSidekick))
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

            TempData["error"] = response?.Message ?? "Course not found";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> Edit(string courseCode)
        {
            var response = await _courseService.GetCourseByCodeAsync(courseCode);

            if (response != null && response.IsSuccess)
            {
                var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(response.Result));

                // Verify the current user is the professor for this course
                var userUniversityId = User.FindFirstValue("UniversityId");
                if (course.ProfessorUniversityId != userUniversityId)
                {
                    TempData["error"] = "You are not authorized to edit this course";
                    return RedirectToAction(nameof(Index));
                }

                var courseEditDto = new CourseCreateDto
                {
                    CourseCode = course.CourseCode,
                    Title = course.Title,
                    Description = course.Description,
                    ProfessorUniversityId = course.ProfessorUniversityId
                };

                return View(courseEditDto);
            }

            TempData["error"] = response?.Message ?? "Course not found";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> Edit(CourseUpdateDto courseDto)
        {
            if (ModelState.IsValid)
            {
                // Verify the current user is the professor for this course
                var userUniversityId = User.FindFirstValue("UniversityId");
                if (courseDto.ProfessorUniversityId != userUniversityId)
                {
                    TempData["error"] = "You are not authorized to edit this course";
                    return RedirectToAction(nameof(Index));
                }

                var response = await _courseService.UpdateCourseAsync(courseDto);

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

            return View(courseDto);
        }

        [HttpPost]
        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> Delete(int id)
        {
            // First verify the course belongs to the current professor
            var courseResponse = await _courseService.GetCourseByIdAsync(id);
            if (courseResponse != null && courseResponse.IsSuccess)
            {
                var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(courseResponse.Result));
                var userUniversityId = User.FindFirstValue("UniversityId");

                if (course.ProfessorUniversityId != userUniversityId)
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
                TempData["error"] = courseResponse?.Message ?? "Course not found";
            }

            return RedirectToAction(nameof(ProfessorCourses));
        }
    }
}