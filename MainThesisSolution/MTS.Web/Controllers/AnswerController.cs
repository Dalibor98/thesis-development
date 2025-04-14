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
    public class AnswerController : Controller
    {
        private readonly IQuizService _quizService;
        private readonly ICourseService _courseService;

        public AnswerController(IQuizService quizService, ICourseService courseService)
        {
            _quizService = quizService;
            _courseService = courseService;
        }

        public async Task<IActionResult> Create(string questionCode)
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

                // Only allow adding answers to multiple-choice questions
                if (quiz.QuizType != "MultipleChoice")
                {
                    TempData["error"] = "Cannot add answers to text-based questions";
                    return RedirectToAction("View", "Quiz", new { quizCode = quiz.QuizCode });
                }

                // Verify ownership through course
                var courseResponse = await _courseService.GetCourseByCodeAsync(quiz.CourseCode);
                if (courseResponse != null && courseResponse.IsSuccess)
                {
                    var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(courseResponse.Result));
                    var userUniversityId = User.FindFirstValue("UniversityId");

                    if (course.ProfessorUniversityId != userUniversityId)
                    {
                        TempData["error"] = "You are not authorized to add answers to this question";
                        return RedirectToAction("Index", "Home");
                    }
                }

                ViewBag.QuestionText = question.QuestionText;
                ViewBag.QuizCode = quiz.QuizCode;
            }

            var model = new AnswerCreateDto
            {
                QuizQuestionCode = questionCode
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AnswerCreateDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _quizService.CreateAnswerAsync(model);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Answer option added successfully";

                    // Get the question to redirect back to quiz
                    var questionResponse = await _quizService.GetQuestionByCodeAsync(model.QuizQuestionCode);
                    if (questionResponse != null && questionResponse.IsSuccess)
                    {
                        var question = JsonConvert.DeserializeObject<QuizQuestionDto>(Convert.ToString(questionResponse.Result));
                        return RedirectToAction("View", "Quiz", new { quizCode = question.QuizCode });
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["error"] = response?.Message ?? "Failed to create answer";
                }
            }

            // Get question for the ViewBag
            var qResponse = await _quizService.GetQuestionByCodeAsync(model.QuizQuestionCode);
            if (qResponse != null && qResponse.IsSuccess)
            {
                var question = JsonConvert.DeserializeObject<QuizQuestionDto>(Convert.ToString(qResponse.Result));
                ViewBag.QuestionText = question.QuestionText;

                var quizResponse = await _quizService.GetQuizByCodeAsync(question.QuizCode);
                if (quizResponse != null && quizResponse.IsSuccess)
                {
                    var quiz = JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(quizResponse.Result));
                    ViewBag.QuizCode = quiz.QuizCode;
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(string answerCode)
        {
            var answersResponse = await _quizService.GetAnswersByCodeAsync(answerCode);
            if (answersResponse == null || !answersResponse.IsSuccess)
            {
                TempData["error"] = "Answer not found";
                return RedirectToAction("Index", "Home");
            }

            var answer = JsonConvert.DeserializeObject<AnswerDto>(Convert.ToString(answersResponse.Result));

            // Get question and quiz for verification
            var questionResponse = await _quizService.GetQuestionByCodeAsync(answer.QuizQuestionCode);
            if (questionResponse != null && questionResponse.IsSuccess)
            {
                var question = JsonConvert.DeserializeObject<QuizQuestionDto>(Convert.ToString(questionResponse.Result));

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
                            TempData["error"] = "You are not authorized to edit this answer";
                            return RedirectToAction("Index", "Home");
                        }
                    }

                    ViewBag.QuestionText = question.QuestionText;
                    ViewBag.QuizCode = quiz.QuizCode;
                }
            }

            var model = new AnswerUpdateDto
            {
                Id = answer.Id,
                QuizQuestionCode = answer.QuizQuestionCode,
                AnswerCode = answer.AnswerCode,
                OptionText = answer.OptionText,
                IsCorrect = answer.IsCorrect
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AnswerUpdateDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _quizService.UpdateAnswerAsync(model);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Answer updated successfully";

                    // Get the question to redirect back to quiz
                    var questionResponse = await _quizService.GetQuestionByCodeAsync(model.QuizQuestionCode);
                    if (questionResponse != null && questionResponse.IsSuccess)
                    {
                        var question = JsonConvert.DeserializeObject<QuizQuestionDto>(Convert.ToString(questionResponse.Result));
                        return RedirectToAction("View", "Quiz", new { quizCode = question.QuizCode });
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["error"] = response?.Message ?? "Failed to update answer";
                }
            }

            // Get question for the ViewBag
            var qResponse = await _quizService.GetQuestionByCodeAsync(model.QuizQuestionCode);
            if (qResponse != null && qResponse.IsSuccess)
            {
                var question = JsonConvert.DeserializeObject<QuizQuestionDto>(Convert.ToString(qResponse.Result));
                ViewBag.QuestionText = question.QuestionText;

                var quizResponse = await _quizService.GetQuizByCodeAsync(question.QuizCode);
                if (quizResponse != null && quizResponse.IsSuccess)
                {
                    var quiz = JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(quizResponse.Result));
                    ViewBag.QuizCode = quiz.QuizCode;
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(string answerCode)
        {
            var answersResponse = await _quizService.GetAnswersByCodeAsync(answerCode);
            if (answersResponse == null || !answersResponse.IsSuccess)
            {
                TempData["error"] = "Answer not found";
                return RedirectToAction("Index", "Home");
            }

            var answer = JsonConvert.DeserializeObject<AnswerDto>(Convert.ToString(answersResponse.Result));

            // Get question and quiz for verification
            var questionResponse = await _quizService.GetQuestionByCodeAsync(answer.QuizQuestionCode);
            if (questionResponse != null && questionResponse.IsSuccess)
            {
                var question = JsonConvert.DeserializeObject<QuizQuestionDto>(Convert.ToString(questionResponse.Result));

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
                            TempData["error"] = "You are not authorized to delete this answer";
                            return RedirectToAction("Index", "Home");
                        }
                    }

                    ViewBag.QuestionText = question.QuestionText;
                    ViewBag.QuizCode = quiz.QuizCode;
                }
            }

            return View(answer);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, string quizCode)
        {
            var response = await _quizService.DeleteAnswerAsync(id);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Answer deleted successfully";
            }
            else
            {
                TempData["error"] = response?.Message ?? "Failed to delete answer";
            }

            return RedirectToAction("View", "Quiz", new { quizCode });
        }
    }
}