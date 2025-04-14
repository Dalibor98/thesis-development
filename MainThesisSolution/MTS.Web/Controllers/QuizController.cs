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

        [HttpGet]
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

        [HttpPost]
        [Authorize(Roles = SD.RoleSidekick)]
        public async Task<IActionResult> StartQuiz(string quizCode)
        {
            return await Take(quizCode);
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

        [Authorize(Roles = SD.RoleSidekick)]
        public async Task<IActionResult> Take(string quizCode)
        {
            var response = await _quizService.GetQuizByCodeAsync(quizCode);

            if (response == null || !response.IsSuccess)
            {
                TempData["error"] = "Quiz not found";
                return RedirectToAction("Index", "Home");
            }

            var quiz = JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(response.Result));

            // Check if quiz is available (within the time window)
            if (DateTime.Now < quiz.StartTime)
            {
                TempData["error"] = "This quiz is not yet available";
                return RedirectToAction("View", new { quizCode });
            }

            if (DateTime.Now > quiz.EndTime)
            {
                TempData["error"] = "This quiz is no longer available";
                return RedirectToAction("View", new { quizCode });
            }

            // Check if the student already has an attempt
            var studentId = User.FindFirstValue("UniversityId");
            var attemptsResponse = await _quizService.GetAttemptsByStudentIdAsync(studentId);

            if (attemptsResponse != null && attemptsResponse.IsSuccess)
            {
                var attempts = JsonConvert.DeserializeObject<List<StudentQuizAttemptDto>>(Convert.ToString(attemptsResponse.Result));
                var existingAttempt = attempts?.FirstOrDefault(a => a.QuizCode == quizCode);

                if (existingAttempt != null)
                {
                    // If an attempt exists, check if it's still in progress
                    if (existingAttempt.EndTime > DateTime.Now)
                    {
                        // Continue the attempt
                        return RedirectToAction("Answer", new { attemptCode = existingAttempt.AttemptCode });
                    }
                    else
                    {
                        TempData["error"] = "You have already completed this quiz";
                        return RedirectToAction("View", new { quizCode });
                    }
                }
            }

            // Get questions for this quiz
            var questionsResponse = await _quizService.GetQuestionsByQuizCodeAsync(quizCode);
            var questions = new List<QuizQuestionDto>();

            if (questionsResponse != null && questionsResponse.IsSuccess)
            {
                questions = JsonConvert.DeserializeObject<List<QuizQuestionDto>>(Convert.ToString(questionsResponse.Result));
            }

            if (!questions.Any())
            {
                TempData["error"] = "This quiz has no questions";
                return RedirectToAction("View", new { quizCode });
            }

            // Create a new attempt
            var attemptCreateDto = new StudentQuizAttemptCreateDto
            {
                QuizCode = quizCode,
                StudentUniversityId = studentId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddMinutes(quiz.TimeLimit)
            };

            var createAttemptResponse = await _quizService.CreateAttemptAsync(attemptCreateDto);

            if (createAttemptResponse == null || !createAttemptResponse.IsSuccess)
            {
                TempData["error"] = "Failed to start quiz";
                return RedirectToAction("View", new { quizCode });
            }

            var attempt = JsonConvert.DeserializeObject<StudentQuizAttemptDto>(Convert.ToString(createAttemptResponse.Result));

            // Redirect to the first question
            return RedirectToAction("Answer", new { attemptCode = attempt.AttemptCode });
        }

        [Authorize(Roles = SD.RoleSidekick)]
        public async Task<IActionResult> Answer(string attemptCode, int? questionIndex = 0)
        {
            // Get the attempt
            var attemptResponse = await _quizService.GetAttemptByCodeAsync(attemptCode);

            if (attemptResponse == null || !attemptResponse.IsSuccess)
            {
                TempData["error"] = "Quiz attempt not found";
                return RedirectToAction("Index", "Home");
            }

            var attempt = JsonConvert.DeserializeObject<StudentQuizAttemptDto>(Convert.ToString(attemptResponse.Result));

            // Verify this attempt belongs to the current student
            var studentId = User.FindFirstValue("UniversityId");
            if (attempt.StudentUniversityId != studentId)
            {
                TempData["error"] = "Unauthorized access";
                return RedirectToAction("Index", "Home");
            }

            // Check if the attempt is still valid (not expired)
            if (DateTime.Now > attempt.EndTime)
            {
                TempData["error"] = "Quiz time expired";
                return RedirectToAction("View", new { quizCode = attempt.QuizCode });
            }

            // Get the quiz details
            var quizResponse = await _quizService.GetQuizByCodeAsync(attempt.QuizCode);
            var quiz = JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(quizResponse.Result));

            // Get questions for this quiz
            var questionsResponse = await _quizService.GetQuestionsByQuizCodeAsync(attempt.QuizCode);
            var questions = new List<QuizQuestionDto>();

            if (questionsResponse != null && questionsResponse.IsSuccess)
            {
                questions = JsonConvert.DeserializeObject<List<QuizQuestionDto>>(Convert.ToString(questionsResponse.Result));
            }

            if (!questions.Any())
            {
                TempData["error"] = "This quiz has no questions";
                return RedirectToAction("View", new { quizCode = attempt.QuizCode });
            }

            // Ensure valid question index
            int index = questionIndex ?? 0;
            if (index < 0 || index >= questions.Count)
            {
                index = 0;
            }

            var currentQuestion = questions[index];

            // Get answers for this question
            var answersResponse = await _quizService.GetAnswersForQuestionAsync(currentQuestion.QuizQuestionCode);
            var answers = new List<AnswerDto>();

            if (answersResponse != null && answersResponse.IsSuccess)
            {
                answers = JsonConvert.DeserializeObject<List<AnswerDto>>(Convert.ToString(answersResponse.Result));
            }

            // Get student's previous answer to this question if any
            var studentAnswersResponse = await _quizService.GetAnswersByAttemptCodeAsync(attemptCode);
            var studentAnswer = new StudentQuizAnswerDto();

            if (studentAnswersResponse != null && studentAnswersResponse.IsSuccess)
            {
                var studentAnswers = JsonConvert.DeserializeObject<List<StudentQuizAnswerDto>>(
                    Convert.ToString(studentAnswersResponse.Result));

                studentAnswer = studentAnswers?.FirstOrDefault(a => a.QuizQuestionCode == currentQuestion.QuizQuestionCode);
            }

            // Prepare the view model
            var model = new TakeQuizViewModel
            {
                Attempt = attempt,
                Quiz = quiz,
                Question = currentQuestion,
                Answers = answers,
                StudentAnswer = studentAnswer,
                QuestionIndex = index,
                TotalQuestions = questions.Count,
                TimeRemaining = (int)Math.Ceiling((attempt.EndTime - DateTime.Now).TotalSeconds)
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = SD.RoleSidekick)]
        public async Task<IActionResult> Answer(StudentQuizAnswerCreateDto answerDto, int questionIndex, int totalQuestions)
        {
            // Save the student's answer
            var response = await _quizService.SaveStudentAnswerAsync(answerDto);

            if (response == null || !response.IsSuccess)
            {
                TempData["error"] = "Failed to save answer";
                return RedirectToAction("Answer", new { attemptCode = answerDto.AttemptCode, questionIndex });
            }

            // Move to the next question or finish
            int nextIndex = questionIndex + 1;
            if (nextIndex < totalQuestions)
            {
                return RedirectToAction("Answer", new { attemptCode = answerDto.AttemptCode, questionIndex = nextIndex });
            }
            else
            {
                return RedirectToAction("Finish", new { attemptCode = answerDto.AttemptCode });
            }
        }
        [Authorize(Roles = SD.RoleSidekick)]
        public async Task<IActionResult> Finish(string attemptCode)
        {
            // Get the attempt
            var attemptResponse = await _quizService.GetAttemptByCodeAsync(attemptCode);

            if (attemptResponse == null || !attemptResponse.IsSuccess)
            {
                TempData["error"] = "Quiz attempt not found";
                return RedirectToAction("Index", "Home");
            }

            var attempt = JsonConvert.DeserializeObject<StudentQuizAttemptDto>(Convert.ToString(attemptResponse.Result));

            // Verify this attempt belongs to the current student
            var studentId = User.FindFirstValue("UniversityId");
            if (attempt.StudentUniversityId != studentId)
            {
                TempData["error"] = "Unauthorized access";
                return RedirectToAction("Index"
                    , "Home");
            }

            // Mark the attempt as completed
            attempt.EndTime = DateTime.Now;

            // Calculate the score
            var scoreResponse = await _quizService.CalculateScoreAsync(attemptCode);

            if (scoreResponse != null && scoreResponse.IsSuccess)
            {
                var score = JsonConvert.DeserializeObject<int>(Convert.ToString(scoreResponse.Result));
                attempt.Score = score;
            }

            // Update the attempt
            var updateResponse = await _quizService.UpdateAttemptAsync(attempt);

            if (updateResponse == null || !updateResponse.IsSuccess)
            {
                TempData["error"] = "Failed to update quiz attempt";
            }
            else
            {
                TempData["success"] = "Quiz completed successfully";
            }

            // Redirect to the quiz results
            return RedirectToAction("Result", new { attemptCode });
        }

        [Authorize(Roles = SD.RoleSidekick)]
        public async Task<IActionResult> Result(string attemptCode)
        {
            // Get the attempt
            var attemptResponse = await _quizService.GetAttemptByCodeAsync(attemptCode);

            if (attemptResponse == null || !attemptResponse.IsSuccess)
            {
                TempData["error"] = "Quiz attempt not found";
                return RedirectToAction("Index", "Home");
            }

            var attempt = JsonConvert.DeserializeObject<StudentQuizAttemptDto>(Convert.ToString(attemptResponse.Result));

            // Verify this attempt belongs to the current student
            var studentId = User.FindFirstValue("UniversityId");
            if (attempt.StudentUniversityId != studentId)
            {
                TempData["error"] = "Unauthorized access";
                return RedirectToAction("Index", "Home");
            }

            // Get the quiz details
            var quizResponse = await _quizService.GetQuizByCodeAsync(attempt.QuizCode);
            var quiz = JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(quizResponse.Result));

            // Get questions and student's answers
            var questionsResponse = await _quizService.GetQuestionsByQuizCodeAsync(attempt.QuizCode);
            var questions = new List<QuizQuestionDto>();

            if (questionsResponse != null && questionsResponse.IsSuccess)
            {
                questions = JsonConvert.DeserializeObject<List<QuizQuestionDto>>(Convert.ToString(questionsResponse.Result));
            }

            var studentAnswersResponse = await _quizService.GetAnswersByAttemptCodeAsync(attemptCode);
            var studentAnswers = new List<StudentQuizAnswerDto>();

            if (studentAnswersResponse != null && studentAnswersResponse.IsSuccess)
            {
                studentAnswers = JsonConvert.DeserializeObject<List<StudentQuizAnswerDto>>(
                    Convert.ToString(studentAnswersResponse.Result));
            }

            // Create a detailed result view model
            var model = new QuizResultViewModel
            {
                Attempt = attempt,
                Quiz = quiz,
                Questions = questions,
                StudentAnswers = studentAnswers
            };

            return View(model);
        }

        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> AttemptDetails(string attemptCode)
        {
            var attemptResponse = await _quizService.GetAttemptByCodeAsync(attemptCode);
            if (attemptResponse == null || !attemptResponse.IsSuccess)
            {
                TempData["error"] = "Attempt not found";
                return RedirectToAction("Index", "Home");
            }

            var attempt = JsonConvert.DeserializeObject<StudentQuizAttemptDto>(Convert.ToString(attemptResponse.Result));

            // Get the quiz
            var quizResponse = await _quizService.GetQuizByCodeAsync(attempt.QuizCode);
            var quiz = JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(quizResponse.Result));

            // Verify ownership
            var courseResponse = await _courseService.GetCourseByCodeAsync(quiz.CourseCode);
            if (courseResponse != null && courseResponse.IsSuccess)
            {
                var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(courseResponse.Result));
                var userUniversityId = User.FindFirstValue("UniversityId");

                if (course.ProfessorUniversityId != userUniversityId)
                {
                    TempData["error"] = "You are not authorized to view this attempt";
                    return RedirectToAction("Index", "Home");
                }
            }

            // Get questions for this quiz
            var questionsResponse = await _quizService.GetQuestionsByQuizCodeAsync(quiz.QuizCode);
            var questions = new List<QuizQuestionDto>();

            if (questionsResponse != null && questionsResponse.IsSuccess)
            {
                questions = JsonConvert.DeserializeObject<List<QuizQuestionDto>>(Convert.ToString(questionsResponse.Result));
            }

            // Get student's answers
            var studentAnswersResponse = await _quizService.GetAnswersByAttemptCodeAsync(attemptCode);
            var studentAnswers = new List<StudentQuizAnswerDto>();

            if (studentAnswersResponse != null && studentAnswersResponse.IsSuccess)
            {
                studentAnswers = JsonConvert.DeserializeObject<List<StudentQuizAnswerDto>>(
                    Convert.ToString(studentAnswersResponse.Result));
            }

            // For each question, get all possible answers (for multiple choice quizzes)
            var questionAnswers = new Dictionary<string, List<AnswerDto>>();

            if (quiz.QuizType == "MultipleChoice")
            {
                foreach (var question in questions)
                {
                    var answersResponse = await _quizService.GetAnswersForQuestionAsync(question.QuizQuestionCode);
                    if (answersResponse != null && answersResponse.IsSuccess)
                    {
                        var answers = JsonConvert.DeserializeObject<List<AnswerDto>>(Convert.ToString(answersResponse.Result));
                        questionAnswers[question.QuizQuestionCode] = answers;
                    }
                    else
                    {
                        questionAnswers[question.QuizQuestionCode] = new List<AnswerDto>();
                    }
                }
            }

            // Create the view model
            var model = new AttemptDetailsViewModel
            {
                Attempt = attempt,
                Quiz = quiz,
                Questions = questions,
                StudentAnswers = studentAnswers,
                QuestionAnswers = questionAnswers
            };

            return View(model);
        }
        [Authorize(Roles = SD.RoleSidekick)]
        public async Task<IActionResult> UpcomingQuizzes()
        {
            var studentId = User.FindFirstValue("UniversityId");
            var response = await _quizService.GetUpcomingQuizzesByStudentIdAsync(studentId);

            if (response != null && response.IsSuccess)
            {
                var quizzes = JsonConvert.DeserializeObject<List<QuizDto>>(
                    Convert.ToString(response.Result));
                return View(quizzes);
            }

            TempData["error"] = "Failed to retrieve upcoming quizzes";
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> RecentAttempts()
        {
            var professorId = User.FindFirstValue("UniversityId");
            var response = await _quizService.GetRecentQuizAttemptsByProfessorIdAsync(professorId);

            if (response != null && response.IsSuccess)
            {
                var attempts = JsonConvert.DeserializeObject<List<StudentQuizAttemptDto>>(
                    Convert.ToString(response.Result));
                return View(attempts);
            }

            TempData["error"] = "Failed to retrieve recent attempts";
            return RedirectToAction("Index", "Home");
        }

        // MTS.Web/Controllers/QuizController.cs - Add GradeAnswer action
        [HttpPost]
        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> GradeAnswer(StudentQuizAnswerGradeDto gradeDto)
        {
            if (ModelState.IsValid)
            {
                var response = await _quizService.GradeStudentAnswerAsync(gradeDto);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Answer graded successfully";
                }
                else
                {
                    TempData["error"] = response?.Message ?? "Failed to grade answer";
                }
            }
            else
            {
                TempData["error"] = "Invalid grade data";
            }

            // Get the attempt code to redirect back to the attempt details
            var studentAnswerResponse = await _quizService.GetStudentAnswerByIdAsync(gradeDto.Id);
            if (studentAnswerResponse != null && studentAnswerResponse.IsSuccess)
            {
                var studentAnswer = JsonConvert.DeserializeObject<StudentQuizAnswerDto>(
                    Convert.ToString(studentAnswerResponse.Result));
                return RedirectToAction("AttemptDetails", new { attemptCode = studentAnswer.AttemptCode });
            }

            // If we couldn't get the attempt code, redirect to the home page
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> GradeTextQuiz(string quizCode)
        {
            // Get the quiz to verify it's text-based
            var quizResponse = await _quizService.GetQuizByCodeAsync(quizCode);
            if (quizResponse == null || !quizResponse.IsSuccess)
            {
                TempData["error"] = "Quiz not found";
                return RedirectToAction("ProfessorDashboard", "Home");
            }

            var quiz = JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(quizResponse.Result));

            // Verify this is a text-based quiz
            if (quiz.QuizType != "TextBased")
            {
                TempData["error"] = "This is not a text-based quiz";
                return RedirectToAction("ProfessorDashboard", "Home");
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
                    return RedirectToAction("ProfessorDashboard", "Home");
                }
            }

            // Get all attempts for this quiz
            var attemptsResponse = await _quizService.GetAttemptsByQuizCodeAsync(quizCode);
            var attempts = new List<StudentQuizAttemptDto>();

            if (attemptsResponse != null && attemptsResponse.IsSuccess)
            {
                attempts = JsonConvert.DeserializeObject<List<StudentQuizAttemptDto>>(
                    Convert.ToString(attemptsResponse.Result));
            }

            // Filter attempts that need grading
            var pendingAttemptsViewModel = new List<AttemptWithStatusViewModel>();

            foreach (var attempt in attempts)
            {
                // Get all answers for this attempt
                var answersResponse = await _quizService.GetAnswersByAttemptCodeAsync(attempt.AttemptCode);
                if (answersResponse != null && answersResponse.IsSuccess)
                {
                    var answers = JsonConvert.DeserializeObject<List<StudentQuizAnswerDto>>(
                        Convert.ToString(answersResponse.Result));

                    // Check if any answer needs grading
                    bool needsGrading = answers.Any(a => !string.IsNullOrEmpty(a.TextAnswer) && a.PointsEarned == 0);

                    pendingAttemptsViewModel.Add(new AttemptWithStatusViewModel
                    {
                        Attempt = attempt,
                        NeedsGrading = needsGrading
                    });
                }
            }

            // Create model for the view
            var model = new GradeTextQuizViewModel
            {
                Quiz = quiz,
                Attempts = pendingAttemptsViewModel
            };

            return View(model);
        }
    }
}