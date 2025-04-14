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
    [Authorize(Roles = SD.RoleLeader)]
    public class QuizQuestionController : Controller
    {
        private readonly IQuizService _quizService;
        private readonly ICourseService _courseService;

        public QuizQuestionController(IQuizService quizService, ICourseService courseService)
        {
            _quizService = quizService;
            _courseService = courseService;
        }

        public async Task<IActionResult> Create(string quizCode)
        {
            var quizResponse = await _quizService.GetQuizByCodeAsync(quizCode);
            if (quizResponse == null || !quizResponse.IsSuccess)
            {
                TempData["error"] = "Quiz not found";
                return RedirectToAction("Index", "Home");
            }

            var quiz = JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(quizResponse.Result));

            // Verify ownership
            var courseResponse = await _courseService.GetCourseByCodeAsync(quiz.CourseCode);
            if (courseResponse != null && courseResponse.IsSuccess)
            {
                var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(courseResponse.Result));
                var userUniversityId = User.FindFirstValue("UniversityId");

                if (course.ProfessorUniversityId != userUniversityId)
                {
                    TempData["error"] = "You are not authorized to add questions to this quiz";
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.QuizTitle = quiz.Title;

            var model = new QuizQuestionCreateDto
            {
                QuizCode = quizCode,
                Points = 10 // Default points
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(QuizQuestionCreateDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _quizService.CreateQuestionAsync(model);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Question added successfully";
                    return RedirectToAction("View", "Quiz", new { quizCode = model.QuizCode });
                }
                else
                {
                    TempData["error"] = response?.Message ?? "Failed to create question";
                }
            }

            // Get quiz for the ViewBag title
            var quizResponse = await _quizService.GetQuizByCodeAsync(model.QuizCode);
            if (quizResponse != null && quizResponse.IsSuccess)
            {
                var quiz = JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(quizResponse.Result));
                ViewBag.QuizTitle = quiz.Title;
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(string questionCode)
        {
            var questionResponse = await _quizService.GetQuestionByCodeAsync(questionCode);
            if (questionResponse == null || !questionResponse.IsSuccess)
            {
                TempData["error"] = "Question not found";
                return RedirectToAction("Index", "Home");
            }

            var question = JsonConvert.DeserializeObject<QuizQuestionDto>(Convert.ToString(questionResponse.Result));

            // Get quiz for verification
            var quizResponse = await _quizService.GetQuizByCodeAsync(question.QuizCode);
            if (quizResponse != null && quizResponse.IsSuccess)
            {
                var quiz = JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(quizResponse.Result));

                // Verify ownership through course
                var courseResponse = await _courseService.GetCourseByCodeAsync(quiz.CourseCode);
                if (courseResponse != null && courseResponse.IsSuccess)
                {
                    var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(courseResponse.Result));
                    var userUniversityId = User.FindFirstValue("UniversityId");

                    if (course.ProfessorUniversityId != userUniversityId)
                    {
                        TempData["error"] = "You are not authorized to edit this question";
                        return RedirectToAction("Index", "Home");
                    }
                }

                ViewBag.QuizTitle = quiz.Title;
            }

            var model = new QuizQuestionUpdateDto
            {
                QuizCode = question.QuizCode,
                QuizQuestionCode = question.QuizQuestionCode,
                QuestionText = question.QuestionText,
                Points = question.Points
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(QuizQuestionUpdateDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _quizService.UpdateQuestionAsync(model);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Question updated successfully";
                    return RedirectToAction("View", "Quiz", new { quizCode = model.QuizCode });
                }
                else
                {
                    TempData["error"] = response?.Message ?? "Failed to update question";
                }
            }

            // Get quiz for the ViewBag title
            var quizResponse = await _quizService.GetQuizByCodeAsync(model.QuizCode);
            if (quizResponse != null && quizResponse.IsSuccess)
            {
                var quiz = JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(quizResponse.Result));
                ViewBag.QuizTitle = quiz.Title;
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(string questionCode)
        {
            var questionResponse = await _quizService.GetQuestionByCodeAsync(questionCode);
            if (questionResponse == null || !questionResponse.IsSuccess)
            {
                TempData["error"] = "Question not found";
                return RedirectToAction("Index", "Home");
            }

            var question = JsonConvert.DeserializeObject<QuizQuestionDto>(Convert.ToString(questionResponse.Result));

            // Get quiz for verification
            var quizResponse = await _quizService.GetQuizByCodeAsync(question.QuizCode);
            if (quizResponse != null && quizResponse.IsSuccess)
            {
                var quiz = JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(quizResponse.Result));

                // Verify ownership through course
                var courseResponse = await _courseService.GetCourseByCodeAsync(quiz.CourseCode);
                if (courseResponse != null && courseResponse.IsSuccess)
                {
                    var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(courseResponse.Result));
                    var userUniversityId = User.FindFirstValue("UniversityId");

                    if (course.ProfessorUniversityId != userUniversityId)
                    {
                        TempData["error"] = "You are not authorized to delete this question";
                        return RedirectToAction("Index", "Home");
                    }
                }

                ViewBag.QuizTitle = quiz.Title;
            }

            return View(question);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string questionCode)
        {
            var questionResponse = await _quizService.GetQuestionByCodeAsync(questionCode);
            if (questionResponse == null || !questionResponse.IsSuccess)
            {
                TempData["error"] = "Question not found";
                return RedirectToAction("Index", "Home");
            }

            var question = JsonConvert.DeserializeObject<QuizQuestionDto>(Convert.ToString(questionResponse.Result));
            string quizCode = question.QuizCode;

            var response = await _quizService.DeleteQuestionAsync(questionCode);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Question deleted successfully";
            }
            else
            {
                TempData["error"] = response?.Message ?? "Failed to delete question";
            }

            return RedirectToAction("View", "Quiz", new { quizCode });
        }
    }
}