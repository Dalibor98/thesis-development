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
        private readonly IStudentAnswerService _studentAnswerService;

        public QuizController(
            IQuizService quizService,
            ICourseService courseService,
            IQuizQuestionService quizQuestionService,
            IAnswerOptionService answerOptionService,
            IStudentQuizAttemptService studentQuizAttemptService,
            IStudentAnswerService studentAnswerService)
        {
            _quizService = quizService;
            _courseService = courseService;
            _quizQuestionService = quizQuestionService;
            _answerOptionService = answerOptionService;
            _studentQuizAttemptService = studentQuizAttemptService;
            _studentAnswerService = studentAnswerService;
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

        public async Task<IActionResult> View(string quizCode)
        {
            var response = await _quizService.GetQuizByCodeAsync(quizCode);

            if (response != null && response.IsSuccess)
            {
                var quiz = JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(response.Result));

                // Get questions for this quiz
                var questionsResponse = await _quizQuestionService.GetQuestionsByQuizCodeAsync(quizCode);
                var questions = new List<QuizQuestionDto>();

                if (questionsResponse != null && questionsResponse.IsSuccess)
                {
                    questions = JsonConvert.DeserializeObject<List<QuizQuestionDto>>(Convert.ToString(questionsResponse.Result));
                    ViewBag.Questions = questions;
                }
                else
                {
                    ViewBag.Questions = new List<QuizQuestionDto>();
                }

                // Check if the quiz has no questions and add warning for professor
                if (User.IsInRole(SD.RoleLeader) && (questions == null || !questions.Any()))
                {
                    ViewBag.NoQuestionsWarning = "This quiz doesn't have any questions yet. Students cannot take a quiz without questions.";
                }
                // Check if student has attempted this quiz
                if (User.IsInRole(SD.RoleSidekick))
                {
                    var studentId = User.FindFirstValue("UniversityId");
                    var attemptsResponse = await _studentQuizAttemptService.GetAttemptsByStudentIdAsync(studentId);

                    if (attemptsResponse != null && attemptsResponse.IsSuccess)
                    {
                        var attempts = JsonConvert.DeserializeObject<List<StudentQuizAttemptDto>>(Convert.ToString(attemptsResponse.Result));
                        var attempt = attempts?.FirstOrDefault(a => a.QuizCode == quizCode);

                        if (attempt != null)
                        {
                            ViewBag.HasAttempted = true;
                            ViewBag.AttemptScore = attempt.Score;
                        }
                        else
                        {
                            ViewBag.HasAttempted = false;
                        }
                    }
                }
                // Get student attempts for professor view
                else if (User.IsInRole(SD.RoleLeader))
                {
                    var attemptsResponse = await _studentQuizAttemptService.GetAttemptsByQuizCodeAsync(quizCode);
                    if (attemptsResponse != null && attemptsResponse.IsSuccess)
                    {
                        ViewBag.Attempts = JsonConvert.DeserializeObject<List<StudentQuizAttemptDto>>(Convert.ToString(attemptsResponse.Result));
                    }
                }

                return View(quiz);
            }

            TempData["error"] = response?.Message ?? "Quiz not found";
            return RedirectToAction("Index", "Home");
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

                    // Verify the quiz type hasn't changed
                    if (quizDto.QuizType != quiz.QuizType)
                    {
                        TempData["error"] = "Quiz type cannot be changed after creation";
                        return View(quizDto);
                    }

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

            // Get questions for this quiz (only fetch once)
            var questionsResponse = await _quizQuestionService.GetQuestionsByQuizCodeAsync(quizCode);
            if (questionsResponse == null || !questionsResponse.IsSuccess)
            {
                TempData["error"] = "Error loading quiz questions";
                return RedirectToAction("View", new { quizCode });
            }
            var questions = JsonConvert.DeserializeObject<List<QuizQuestionDto>>(Convert.ToString(questionsResponse.Result));

            // Check if the quiz has any questions
            if (questions == null || !questions.Any())
            {
                TempData["error"] = "This quiz does not have any questions and cannot be taken";
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

            // Calculate adjusted time limit for late starters
            int adjustedTimeLimit = quiz.TimeLimit;
            if (now > quiz.StartTime)
            {
                // Calculate remaining time until quiz end
                var remainingMinutes = (quiz.EndTime - now).TotalMinutes;

                // If the remaining time is less than the quiz's time limit,
                // adjust the time limit to the remaining time
                if (remainingMinutes < quiz.TimeLimit)
                {
                    adjustedTimeLimit = (int)Math.Floor(remainingMinutes);

                    // Ensure we have at least 1 minute
                    adjustedTimeLimit = Math.Max(1, adjustedTimeLimit);
                }
            }

            // Create a new attempt
            var attempt = new StudentQuizAttemptCreateDto
            {
                QuizCode = quizCode,
                StudentUniversityId = studentId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddMinutes(adjustedTimeLimit), // Use adjusted time limit
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

            // Pass the adjusted time limit to the view for displaying in the timer
            ViewBag.AdjustedTimeLimit = adjustedTimeLimit;

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = SD.RoleSidekick)]
        public async Task<IActionResult> SubmitQuiz(string attemptCode, string quizCode, List<StudentAnswerCreateDto> answers)
        {
            if (answers == null || !answers.Any())
            {
                TempData["error"] = "No answers submitted";
                return RedirectToAction("View", new { quizCode });
            }

            // Get the student ID
            var studentId = User.FindFirstValue("UniversityId");

            // Get the attempt to verify it belongs to the student
            var attemptResponse = await _studentQuizAttemptService.GetAttemptByCodeAsync(attemptCode);
            if (attemptResponse == null || !attemptResponse.IsSuccess)
            {
                TempData["error"] = "Quiz attempt not found";
                return RedirectToAction("View", new { quizCode });
            }

            var attempt = JsonConvert.DeserializeObject<StudentQuizAttemptDto>(Convert.ToString(attemptResponse.Result));
            if (attempt.StudentUniversityId != studentId)
            {
                TempData["error"] = "Unauthorized access to quiz attempt";
                return RedirectToAction("View", new { quizCode });
            }

            // Submit each answer
            foreach (var answer in answers)
            {
                if (!string.IsNullOrEmpty(answer.QuizQuestionCode))
                {
                    // Ensure the attempt code is set correctly
                    answer.AttemptCode = attemptCode;

                    // Create/submit the answer
                    await _studentAnswerService.CreateStudentAnswerAsync(answer);
                }
            }

            // Update the attempt end time and calculate score
            var updateAttempt = new StudentQuizAttemptUpdateDto
            {
                Id = attempt.Id,
                AttemptCode = attemptCode,
                EndTime = DateTime.Now,
                Score = 0 // Will be calculated by the API
            };

            await _studentQuizAttemptService.UpdateAttemptAsync(updateAttempt);

            // Trigger score calculation
            await _studentQuizAttemptService.CalculateAndUpdateScoreAsync(attemptCode);

            TempData["success"] = "Quiz submitted successfully";
            return RedirectToAction("View", new { quizCode });
        }

        [Authorize]
        public async Task<IActionResult> AttemptDetails(string attemptCode)
        {
            var attemptResponse = await _studentQuizAttemptService.GetAttemptByCodeAsync(attemptCode);
            if (attemptResponse == null || !attemptResponse.IsSuccess)
            {
                TempData["error"] = "Quiz attempt not found";
                return RedirectToAction("Index", "Home");
            }

            var attempt = JsonConvert.DeserializeObject<StudentQuizAttemptDto>(Convert.ToString(attemptResponse.Result));

            // Get the quiz details
            var quizResponse = await _quizService.GetQuizByCodeAsync(attempt.QuizCode);
            if (quizResponse == null || !quizResponse.IsSuccess)
            {
                TempData["error"] = "Quiz not found";
                return RedirectToAction("Index", "Home");
            }

            var quiz = JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(quizResponse.Result));

            // Verify permissions
            bool isAuthorized = false;
            if (User.IsInRole(SD.RoleLeader))
            {
                // For professors, check if they own the course
                var courseResponse = await _courseService.GetCourseByCodeAsync(quiz.CourseCode);
                if (courseResponse != null && courseResponse.IsSuccess)
                {
                    var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(courseResponse.Result));
                    var userUniversityId = User.FindFirstValue("UniversityId");
                    isAuthorized = course.ProfessorUniversityId == userUniversityId;
                }
            }
            else if (User.IsInRole(SD.RoleSidekick))
            {
                // For students, check if this is their attempt
                var userUniversityId = User.FindFirstValue("UniversityId");
                isAuthorized = attempt.StudentUniversityId == userUniversityId;
            }

            if (!isAuthorized)
            {
                TempData["error"] = "You are not authorized to view this attempt";
                return RedirectToAction("Index", "Home");
            }

            // Get all answers for this attempt
            var answersResponse = await _studentAnswerService.GetAnswersByAttemptCodeAsync(attemptCode);
            var answers = new List<StudentAnswerDto>();
            if (answersResponse != null && answersResponse.IsSuccess)
            {
                answers = JsonConvert.DeserializeObject<List<StudentAnswerDto>>(Convert.ToString(answersResponse.Result));
            }

            // Get question details for each answer
            var questionResponses = new List<QuizQuestionWithOptionsDto>();
            foreach (var answer in answers)
            {
                var questionResponse = await _quizQuestionService.GetQuestionByCodeAsync(answer.QuizQuestionCode);
                if (questionResponse != null && questionResponse.IsSuccess)
                {
                    var question = JsonConvert.DeserializeObject<QuizQuestionDto>(Convert.ToString(questionResponse.Result));

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

                    questionResponses.Add(questionWithOptions);
                }
            }

            ViewBag.Quiz = quiz;
            ViewBag.Attempt = attempt;
            ViewBag.QuestionResponses = questionResponses;

            return View(answers);
        }

        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> GradeTextQuiz(string quizCode)
        {
            var quizResponse = await _quizService.GetQuizByCodeAsync(quizCode);
            if (quizResponse == null || !quizResponse.IsSuccess)
            {
                TempData["error"] = "Quiz not found";
                return RedirectToAction("Index", "Home");
            }

            var quiz = JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(quizResponse.Result));

            // Verify it's a text-based quiz
            if (quiz.QuizType != "TextBased")
            {
                TempData["error"] = "This function is only available for text-based quizzes";
                return RedirectToAction("View", new { quizCode });
            }

            // Verify ownership
            var courseResponse = await _courseService.GetCourseByCodeAsync(quiz.CourseCode);
            if (courseResponse != null && courseResponse.IsSuccess)
            {
                var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(courseResponse.Result));
                var userUniversityId = User.FindFirstValue("UniversityId");

                if (course.ProfessorUniversityId != userUniversityId)
                {
                    TempData["error"] = "You are not authorized to grade this quiz";
                    return RedirectToAction("Index", "Home");
                }
            }

            // Get all attempts for this quiz
            var attemptsResponse = await _studentQuizAttemptService.GetAttemptsByQuizCodeAsync(quizCode);
            var attempts = new List<StudentQuizAttemptDto>();
            if (attemptsResponse != null && attemptsResponse.IsSuccess)
            {
                attempts = JsonConvert.DeserializeObject<List<StudentQuizAttemptDto>>(Convert.ToString(attemptsResponse.Result));
            }

            // Get ungraded answers
            var ungradedAnswers = new List<StudentAnswerDto>();
            foreach (var attempt in attempts)
            {
                var answersResponse = await _studentAnswerService.GetAnswersByAttemptCodeAsync(attempt.AttemptCode);
                if (answersResponse != null && answersResponse.IsSuccess)
                {
                    var answers = JsonConvert.DeserializeObject<List<StudentAnswerDto>>(Convert.ToString(answersResponse.Result));
                    ungradedAnswers.AddRange(answers.Where(a => a.GradingStatus == "Ungraded"));
                }
            }

            // Get question details for ungraded answers
            var questionDetails = new Dictionary<string, QuizQuestionDto>();
            foreach (var answer in ungradedAnswers)
            {
                if (!questionDetails.ContainsKey(answer.QuizQuestionCode))
                {
                    var questionResponse = await _quizQuestionService.GetQuestionByCodeAsync(answer.QuizQuestionCode);
                    if (questionResponse != null && questionResponse.IsSuccess)
                    {
                        var question = JsonConvert.DeserializeObject<QuizQuestionDto>(Convert.ToString(questionResponse.Result));
                        questionDetails[answer.QuizQuestionCode] = question;
                    }
                }
            }

            ViewBag.Quiz = quiz;
            ViewBag.UngradedAnswers = ungradedAnswers;
            ViewBag.QuestionDetails = questionDetails;
            ViewBag.Attempts = attempts;

            // This is the critical fix: pass the quiz model to the view
            return View(quiz);
        }

        [HttpPost]
        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> GradeAnswer(StudentAnswerGradeDto gradeDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Invalid grading data";
                return RedirectToAction("Index", "Home");
            }

            var response = await _studentAnswerService.GradeStudentAnswerAsync(gradeDto);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Answer graded successfully";

                // Get the answer to find the quiz
                var answerResponse = await _studentAnswerService.GetAnswerByIdAsync(gradeDto.Id);
                if (answerResponse != null && answerResponse.IsSuccess)
                {
                    var answer = JsonConvert.DeserializeObject<StudentAnswerDto>(Convert.ToString(answerResponse.Result));

                    // Get the attempt to find the quiz
                    var attemptResponse = await _studentQuizAttemptService.GetAttemptByCodeAsync(answer.AttemptCode);
                    if (attemptResponse != null && attemptResponse.IsSuccess)
                    {
                        var attempt = JsonConvert.DeserializeObject<StudentQuizAttemptDto>(Convert.ToString(attemptResponse.Result));

                        // Recalculate the score for this attempt
                        await _studentQuizAttemptService.CalculateAndUpdateScoreAsync(attempt.AttemptCode);

                        // Redirect back to grading page for this quiz
                        return RedirectToAction("GradeTextQuiz", new { quizCode = attempt.QuizCode });
                    }
                }
            }
            else
            {
                TempData["error"] = response?.Message ?? "Failed to grade answer";
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = SD.RoleSidekick)]
        public async Task<IActionResult> ReviewAttempt(string quizCode)
        {
            var studentId = User.FindFirstValue("UniversityId");

            // Get the student's attempt for this quiz
            var attemptsResponse = await _studentQuizAttemptService.GetAttemptsByStudentIdAsync(studentId);
            if (attemptsResponse == null || !attemptsResponse.IsSuccess)
            {
                TempData["error"] = "No quiz attempts found";
                return RedirectToAction("View", new { quizCode });
            }

            var attempts = JsonConvert.DeserializeObject<List<StudentQuizAttemptDto>>(Convert.ToString(attemptsResponse.Result));
            var attempt = attempts.FirstOrDefault(a => a.QuizCode == quizCode);

            if (attempt == null)
            {
                TempData["error"] = "You have not attempted this quiz";
                return RedirectToAction("View", new { quizCode });
            }

            // Redirect to the attempt details
            return RedirectToAction("AttemptDetails", new { attemptCode = attempt.AttemptCode });
        }
    }
}