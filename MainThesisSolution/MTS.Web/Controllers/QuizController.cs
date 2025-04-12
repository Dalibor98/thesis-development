using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTS.Web.Models.Curriculum.Course;
using MTS.Web.Models.Curriculum.Quiz;
using MTS.Web.Service.IService;
using MTS.Web.Utility;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MTS.Web.Controllers
{
    public class QuizController : Controller
    {
        private readonly IQuizService _quizService;
        private readonly ICourseService _courseService;

        public QuizController(IQuizService quizService, ICourseService courseService)
        {
            _quizService = quizService;
            _courseService = courseService;
        }

        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> Create(string weekCode, string courseCode)
        {
            // Verify that the week belongs to a course owned by the professor
            var courseResponse = await _courseService.GetCourseByCodeAsync(courseCode);
            if (courseResponse == null || !courseResponse.IsSuccess)
            {
                TempData["error"] = "Course not found";
                return RedirectToAction("Index", "Course");
            }

            var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(courseResponse.Result));

            // Verify the current user is the professor for this course
            var userUniversityId = User.FindFirstValue("UniversityId");
            if (course.ProfessorUniversityId != userUniversityId)
            {
                TempData["error"] = "You are not authorized to add quizzes to this course";
                return RedirectToAction("Details", "Course", new { courseCode });
            }

            var quizCreateDto = new QuizCreateDto
            {
                WeekCode = weekCode,
                CourseCode = courseCode,
                StartTime = DateTime.Now,
                TimeLimit = 60 // Default 60 minutes
            };

            return View(quizCreateDto);
        }

        [HttpPost]
        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> Create(QuizCreateDto quizDto)
        {
            if (ModelState.IsValid)
            {
                // Calculate end time based on start time and time limit
                quizDto.EndTime = quizDto.StartTime.AddMinutes(quizDto.TimeLimit);

                // Verify ownership of the course
                var courseResponse = await _courseService.GetCourseByCodeAsync(quizDto.CourseCode);
                if (courseResponse != null && courseResponse.IsSuccess)
                {
                    var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(courseResponse.Result));
                    var userUniversityId = User.FindFirstValue("UniversityId");

                    if (course.ProfessorUniversityId != userUniversityId)
                    {
                        TempData["error"] = "You are not authorized to add quizzes to this course";
                        return RedirectToAction("Details", "Course", new { courseCode = quizDto.CourseCode });
                    }

                    var response = await _quizService.CreateQuizAsync(quizDto);

                    if (response != null && response.IsSuccess)
                    {
                        TempData["success"] = "Quiz created successfully";
                        return RedirectToAction("Details", "Course", new { courseCode = quizDto.CourseCode });
                    }
                    else
                    {
                        TempData["error"] = response?.Message;
                    }
                }
            }

            return View(quizDto);
        }

        public async Task<IActionResult> View(string quizCode)
        {
            var response = await _quizService.GetQuizByCodeAsync(quizCode);

            if (response != null && response.IsSuccess)
            {
                var quiz = JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(response.Result));

                // For professors, load questions too
                if (User.IsInRole(SD.RoleLeader))
                {
                    var questionsResponse = await _quizService.GetQuestionsByQuizCodeAsync(quizCode);
                    if (questionsResponse != null && questionsResponse.IsSuccess)
                    {
                        ViewBag.Questions = JsonConvert.DeserializeObject<List<QuizQuestionDto>>(
                            Convert.ToString(questionsResponse.Result));
                    }
                    else
                    {
                        ViewBag.Questions = new List<QuizQuestionDto>();
                    }

                    // Get a course so we can check ownership for edit permissions
                    var courseResponse = await _courseService.GetCourseByCodeAsync(quiz.CourseCode);
                    if (courseResponse != null && courseResponse.IsSuccess)
                    {
                        ViewBag.Course = JsonConvert.DeserializeObject<CourseDto>(
                            Convert.ToString(courseResponse.Result));
                    }
                }

                return View(quiz);
            }

            TempData["error"] = response?.Message ?? "Quiz not found";
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> Edit(string quizCode)
        {
            var response = await _quizService.GetQuizByCodeAsync(quizCode);

            if (response != null && response.IsSuccess)
            {
                var quiz = JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(response.Result));

                // Get course info to verify ownership
                var courseResponse = await _courseService.GetCourseByCodeAsync(quiz.CourseCode);
                if (courseResponse != null && courseResponse.IsSuccess)
                {
                    var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(courseResponse.Result));

                    // Verify the current user is the professor for this course
                    var userUniversityId = User.FindFirstValue("UniversityId");
                    if (course.ProfessorUniversityId != userUniversityId)
                    {
                        TempData["error"] = "You are not authorized to edit this quiz";
                        return RedirectToAction("Details", "Course", new { courseCode = quiz.CourseCode });
                    }

                    var quizUpdateDto = new QuizUpdateDto
                    {
                        QuizCode = quiz.QuizCode,
                        CourseCode = quiz.CourseCode,
                        WeekCode = quiz.WeekCode,
                        Title = quiz.Title,
                        StartTime = quiz.StartTime,
                        TimeLimit = quiz.TimeLimit
                        // EndTime removed
                    };

                    return View(quizUpdateDto);
                }
            }

            TempData["error"] = response?.Message ?? "Quiz not found";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> Edit(QuizUpdateDto quizDto)
        {
            if (ModelState.IsValid)
            {
                // Calculate end time based on start time and time limit
                quizDto.EndTime = quizDto.StartTime.AddMinutes(quizDto.TimeLimit);

                // Verify ownership (similar to above)
                var quizResponse = await _quizService.GetQuizByCodeAsync(quizDto.QuizCode);
                if (quizResponse != null && quizResponse.IsSuccess)
                {
                    var quiz = JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(quizResponse.Result));
                    var courseResponse = await _courseService.GetCourseByCodeAsync(quiz.CourseCode);

                    if (courseResponse != null && courseResponse.IsSuccess)
                    {
                        var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(courseResponse.Result));
                        var userUniversityId = User.FindFirstValue("UniversityId");

                        if (course.ProfessorUniversityId != userUniversityId)
                        {
                            TempData["error"] = "You are not authorized to edit this quiz";
                            return RedirectToAction("Details", "Course", new { courseCode = quiz.CourseCode });
                        }

                        var response = await _quizService.UpdateQuizAsync(quizDto);

                        if (response != null && response.IsSuccess)
                        {
                            TempData["success"] = "Quiz updated successfully";
                            return RedirectToAction("Details", "Course", new { courseCode = quiz.CourseCode });
                        }
                        else
                        {
                            TempData["error"] = response?.Message;
                        }
                    }
                }
            }

            return View(quizDto);
        }

        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> Delete(string quizCode)
        {
            var response = await _quizService.GetQuizByCodeAsync(quizCode);

            if (response != null && response.IsSuccess)
            {
                var quiz = JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(response.Result));

                // Verify ownership
                var courseResponse = await _courseService.GetCourseByCodeAsync(quiz.CourseCode);
                if (courseResponse != null && courseResponse.IsSuccess)
                {
                    var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(courseResponse.Result));

                    // Verify the current user is the professor for this course
                    var userUniversityId = User.FindFirstValue("UniversityId");
                    if (course.ProfessorUniversityId != userUniversityId)
                    {
                        TempData["error"] = "You are not authorized to delete this quiz";
                        return RedirectToAction("Details", "Course", new { courseCode = quiz.CourseCode });
                    }

                    return View(quiz);
                }
            }

            TempData["error"] = response?.Message ?? "Quiz not found";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> DeleteConfirmed(string quizCode)
        {
            var quizResponse = await _quizService.GetQuizByCodeAsync(quizCode);
            if (quizResponse != null && quizResponse.IsSuccess)
            {
                var quiz = JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(quizResponse.Result));

                // Store course code for redirect
                string courseCode = quiz.CourseCode;

                var response = await _quizService.DeleteQuizAsync(quizCode);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Quiz deleted successfully";
                }
                else
                {
                    TempData["error"] = response?.Message;
                }

                return RedirectToAction("Details", "Course", new { courseCode });
            }

            TempData["error"] = "Error retrieving quiz details";
            return RedirectToAction("Index", "Home");
        }

        // For professors to see all attempts for a quiz
        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> Attempts(string quizCode)
        {
            var quizResponse = await _quizService.GetQuizByCodeAsync(quizCode);
            if (quizResponse == null || !quizResponse.IsSuccess)
            {
                TempData["error"] = "Quiz not found";
                return RedirectToAction("Index", "Home");
            }

            var quiz = JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(quizResponse.Result));
            ViewBag.Quiz = quiz;

            // Verify the current user is the professor for this course
            var courseResponse = await _courseService.GetCourseByCodeAsync(quiz.CourseCode);
            if (courseResponse != null && courseResponse.IsSuccess)
            {
                var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(courseResponse.Result));
                var userUniversityId = User.FindFirstValue("UniversityId");

                if (course.ProfessorUniversityId != userUniversityId)
                {
                    TempData["error"] = "You are not authorized to view attempts for this quiz";
                    return RedirectToAction("Details", "Course", new { courseCode = quiz.CourseCode });
                }
            }

            var attemptsResponse = await _quizService.GetAttemptsByQuizCodeAsync(quizCode);
            List<StudentQuizAttemptDto> attempts = new();

            if (attemptsResponse != null && attemptsResponse.IsSuccess)
            {
                attempts = JsonConvert.DeserializeObject<List<StudentQuizAttemptDto>>(
                    Convert.ToString(attemptsResponse.Result));
            }

            return View(attempts);
        }
    }
}