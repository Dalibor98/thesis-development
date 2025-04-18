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
    [Authorize]
    public class QuizController : Controller
    {
        private readonly IQuizService _quizService;
        private readonly ICourseService _courseService;
        private readonly IQuizQuestionService _quizQuestionService;
        private readonly IAnswerOptionService _answerOptionService;
        private readonly IStudentQuizAttemptService _studentQuizAttemptService;

        public QuizController(
            IQuizService quizService,
            ICourseService courseService,
            IQuizQuestionService quizQuestionService,
            IAnswerOptionService answerOptionService,
            IStudentQuizAttemptService studentQuizAttemptService)
        {
            _quizService = quizService;
            _courseService = courseService;
            _quizQuestionService = quizQuestionService;
            _answerOptionService = answerOptionService;
            _studentQuizAttemptService = studentQuizAttemptService;
        }

        // GET: Quiz/Create
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
                EndTime = DateTime.Now.AddDays(7),
                TimeLimit = 60 // Default 60 minutes
            };

            return View(quizCreateDto);
        }

        // POST: Quiz/Create
        [HttpPost]
        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> Create(QuizCreateDto quizDto)
        {
            if (ModelState.IsValid)
            {
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
                        var quiz = JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(response.Result));
                        TempData["success"] = "Quiz created successfully";
                        return RedirectToAction("View", new { quizCode = quiz.QuizCode });
                    }
                    else
                    {
                        TempData["error"] = response?.Message;
                    }
                }
            }

            return View(quizDto);
        }



        // GET: Quiz/Edit/quizCode
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
                        EndTime = quiz.EndTime,
                        TimeLimit = quiz.TimeLimit,
                        QuizType = quiz.QuizType
                    };

                    return View(quizUpdateDto);
                }
            }

            TempData["error"] = response?.Message ?? "Quiz not found";
            return RedirectToAction("Index", "Home");
        }

        // POST: Quiz/Edit
        [HttpPost]
        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> Edit(QuizUpdateDto quizDto)
        {
            if (ModelState.IsValid)
            {
                // Verify ownership
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
                            return RedirectToAction("View", new { quizCode = quizDto.QuizCode });
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

        // POST: Quiz/Delete
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
        public async Task<IActionResult> QuestionCreated(string questionCode, bool addAnswers = false)
        {
            if (string.IsNullOrEmpty(questionCode))
            {
                return RedirectToAction("Index", "Home");
            }

            var questionResponse = await _quizQuestionService.GetQuestionByCodeAsync(questionCode);
            if (questionResponse != null && questionResponse.IsSuccess)
            {
                var question = JsonConvert.DeserializeObject<QuizQuestionDto>(Convert.ToString(questionResponse.Result));

                // If this is a multiple-choice question and addAnswers is true, redirect to add answers
                if (addAnswers && question.QuestionType == "MultipleChoice")
                {
                    return RedirectToAction("Create", "Answer", new { questionCode = questionCode });
                }

                // Otherwise, redirect back to the quiz
                return RedirectToAction("View", new { quizCode = question.QuizCode });
            }

            TempData["error"] = "Question not found";
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = SD.RoleSidekick)]
        public async Task<IActionResult> TakeQuiz(string quizCode)
        {
            var quizResponse = await _quizService.GetQuizByCodeAsync(quizCode);
            if (quizResponse == null || !quizResponse.IsSuccess)
            {
                TempData["error"] = "Quiz not found";
                return RedirectToAction("Index", "Home");
            }
            var quiz = JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(quizResponse.Result));

            // Check if the quiz is available
            var now = DateTime.Now;
            if (now < quiz.StartTime)
            {
                TempData["error"] = "This quiz is not yet available";
                return RedirectToAction("View", new { quizCode });
            }
            if (now > quiz.EndTime)
            {
                TempData["error"] = "This quiz is closed";
                return RedirectToAction("View", new { quizCode });
            }

            // Check if the student has already attempted this quiz
            var studentId = User.FindFirstValue("UniversityId");
            var attemptResponse = await _studentQuizAttemptService.GetAttemptsByStudentIdAsync(studentId);
            if (attemptResponse != null && attemptResponse.IsSuccess)
            {
                var attempts = JsonConvert.DeserializeObject<List<StudentQuizAttemptDto>>(Convert.ToString(attemptResponse.Result));
                if (attempts != null && attempts.Any(a => a.QuizCode == quizCode))
                {
                    TempData["error"] = "You have already taken this quiz";
                    return RedirectToAction("View", new { quizCode });
                }
            }

            // Get questions for this quiz
            var questionsResponse = await _quizQuestionService.GetQuestionsByQuizCodeAsync(quizCode);
            if (questionsResponse == null || !questionsResponse.IsSuccess)
            {
                TempData["error"] = "Error loading quiz questions";
                return RedirectToAction("View", new { quizCode });
            }
            var questions = JsonConvert.DeserializeObject<List<QuizQuestionDto>>(Convert.ToString(questionsResponse.Result));

            // Create a new attempt
            var attempt = new StudentQuizAttemptCreateDto
            {
                QuizCode = quizCode,
                StudentUniversityId = studentId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddMinutes(quiz.TimeLimit), // Set end time based on time limit
                Score = 0 // Initial score is 0
            };

            var createAttemptResponse = await _studentQuizAttemptService.CreateAttemptAsync(attempt);
            if (createAttemptResponse == null || !createAttemptResponse.IsSuccess)
            {
                TempData["error"] = "Error creating quiz attempt";
                return RedirectToAction("View", new { quizCode });
            }
            var createdAttempt = JsonConvert.DeserializeObject<StudentQuizAttemptDto>(Convert.ToString(createAttemptResponse.Result));

            // Create the view model
            var viewModel = new QuizTakingViewModel
            {
                Quiz = quiz,
                Attempt = createdAttempt,
                Questions = new List<QuizQuestionWithOptionsDto>()
            };

            // Populate questions with options for multiple-choice questions
            foreach (var question in questions)
            {
                var questionWithOptions = new QuizQuestionWithOptionsDto
                {
                    Question = question,
                    Options = new List<AnswerOptionDto>()
                };

                if (quiz.QuizType == "MultipleChoice")
                {
                    var optionsResponse = await _answerOptionService.GetOptionsByQuestionCodeAsync(question.QuizQuestionCode);
                    if (optionsResponse != null && optionsResponse.IsSuccess)
                    {
                        questionWithOptions.Options = JsonConvert.DeserializeObject<List<AnswerOptionDto>>(Convert.ToString(optionsResponse.Result));
                    }
                }

                viewModel.Questions.Add(questionWithOptions);
            }

            return View(viewModel);
        }
    }
}