using Microsoft.AspNetCore.Mvc;
using MTS.Web.Models;
using MTS.Web.Models.Admin;
using MTS.Web.Models.Auth;
using MTS.Web.Models.Curriculum.Course;
using MTS.Web.Models.Curriculum.Quiz;
using MTS.Web.Models.User.Professor;
using MTS.Web.Models.User.Student;
using MTS.Web.Models.User.UniId;
using MTS.Web.Service;
using MTS.Web.Service.IService;
using MTS.Web.Utility;
using Newtonsoft.Json;

namespace MTS.Web.Controllers
{
    public class AdminController : Controller
    {

        private readonly IAdminService _adminService;
        private readonly IProfessorService _professorService;
        private readonly IAuthService _authService;
        private readonly ICourseService _courseService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IQuizService _quizService;
        private readonly IStudentQuizAttemptService _studentQuizAttemptService;
        private readonly IStudentAnswerService _studentAnswerService;
        private readonly IQuizQuestionService _quizQuestionService;
        private readonly IAnswerOptionService _answerOptionService;

        public AdminController(
            IAdminService adminService,
            IProfessorService professorService,
            IAuthService authService,
            ICourseService courseService,
            IEnrollmentService enrollmentService,
            IQuizService quizService,
            IStudentQuizAttemptService studentQuizAttemptService,
            IStudentAnswerService studentAnswerService,
            IQuizQuestionService quizQuestionService,
            IAnswerOptionService answerOptionService)
        {
            _adminService = adminService;
            _professorService = professorService;
            _authService = authService;
            _courseService = courseService;
            _enrollmentService = enrollmentService;
            _quizService = quizService;
            _studentQuizAttemptService = studentQuizAttemptService;
            _studentAnswerService = studentAnswerService;
            _quizQuestionService = quizQuestionService;
            _answerOptionService = answerOptionService;
        }

        public async Task<IActionResult> Index()
        {
            await EnsureAdminUserExists();
            return View();
        }
        private async Task EnsureAdminUserExists()
        {
            // Check if admin role exists and create it if needed
            var adminRoleDto = new RegistrationRequestDto
            {
                Email = "admin@university.edu",
                Name = "System Administrator",
                Password = "admin",
                Role = SD.RoleAdmin
            };

            // Create the admin user if it doesn't exist
            var loginResponse = await _authService.LoginAsync(new LoginRequestDto { UserName = "admin@university.edu", Password = "admin" });

            if (loginResponse == null || !loginResponse.IsSuccess)
            {
                // User doesn't exist, create it
                var registrationResponse = await _authService.RegisterAsync(adminRoleDto);

                if (registrationResponse != null && registrationResponse.IsSuccess)
                {
                    // Assign admin role
                    await _authService.AssignRoleAsync(adminRoleDto);
                }
            }
        }

        [HttpGet]
        public IActionResult UniversityIdGenerate()
        {
            UniversityIdGenerateDto universityIdGenerateDto = new();
            return View(universityIdGenerateDto);

        }

        [HttpPost]
        public async Task<IActionResult> UniversityIdGenerate(UniversityIdGenerateDto obj)
        {
            ResponseDto responseDto = await _adminService.GenerateIds(obj);

            if (responseDto != null && responseDto.IsSuccess)
            {
                var generatedIds = JsonConvert.DeserializeObject<List<string>>(responseDto.Result.ToString());
                return View("GeneratedIds", generatedIds);
            }
            else
            {
                TempData["error"] = responseDto.Message;
                return View(obj);
            }
        }

        [HttpGet]
        public IActionResult Admin()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> StudentIndex()
        {
            List<StudentDto>? list = new();
            ResponseDto? response = await _adminService.GetStudentsAsync();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<StudentDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(list);
        }


        public async Task<IActionResult> StudentEdit(int studentId)
        {
            ResponseDto? response = await _adminService.GetStudentByIdAsync(studentId);
            if (response != null && response.IsSuccess)
            {
                StudentDto? model = JsonConvert.DeserializeObject<StudentDto>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> StudentEdit(StudentDto studentDto)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _adminService.UpdateStudentAsync(studentDto);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product updated successfully";
                    return RedirectToAction(nameof(StudentIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(studentDto);
        }

        [HttpGet]
        public async Task<IActionResult> ProfessorIndex()
        {
            List<ProfessorDto>? list = new();
            ResponseDto? response = await _adminService.GetProfessorsAsync();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProfessorDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(list);
        }
        
        public async Task<IActionResult> StudentDelete(int studentId)
        {
            ResponseDto? response = await _adminService.GetStudentByIdAsync(studentId);

            if (response != null && response.IsSuccess)
            {
                StudentDto? model = JsonConvert.DeserializeObject<StudentDto>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }

        [HttpPost]
        [ActionName("StudentDelete")]
        public async Task<IActionResult> StudentDeleteConfirmed(int studentId,string email)
        {
            ResponseDto? userDeleted = await _authService.DeleteAsync(email);

            if (userDeleted != null && userDeleted.IsSuccess)
            {
                ResponseDto? studentDeleted = await _adminService.DeleteStudentAsync(studentId);
                if (studentDeleted != null && studentDeleted.IsSuccess)
                {
                    TempData["success"] = "Student deleted successfully";
                    return RedirectToAction(nameof(StudentIndex));
                }
                TempData["fail"] = studentDeleted?.Message;
                return RedirectToAction(nameof(StudentIndex));
            }
            else
            {
                TempData["error"] = userDeleted?.Message;
            }
            return RedirectToAction(nameof(StudentIndex));
        }

        public async Task<IActionResult> ProfessorEdit(int professorId)
        {
            ResponseDto? response = await _professorService.GetProfessorByIdAsync(professorId);
            if (response != null && response.IsSuccess)
            {
                ProfessorDto? model = JsonConvert.DeserializeObject<ProfessorDto>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ProfessorEdit(ProfessorDto professorDto)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _professorService.UpdateProfessorAsync(professorDto);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product updated successfully";
                    return RedirectToAction(nameof(ProfessorIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(professorDto);
        }

        public async Task<IActionResult> ProfessorDelete(int professorId)
        {
            ResponseDto? response = await _professorService.GetProfessorByIdAsync(professorId);

            if (response != null && response.IsSuccess)
            {
                ProfessorDto? model = JsonConvert.DeserializeObject<ProfessorDto>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }

        [HttpPost]
        [ActionName("ProfessorDelete")]
        public async Task<IActionResult> ProfessortDeleteConfirmed(int professorId)
        {
            ResponseDto? response = await _professorService.DeleteProfessorAsync(professorId);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Student deleted successfully";
                return RedirectToAction(nameof(ProfessorIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return RedirectToAction(nameof(ProfessorIndex));
        }

        [HttpGet]
        public async Task<IActionResult> SystemMetrics()
        {
            // Fetch student count
            ResponseDto? studentResponse = await _adminService.GetStudentsAsync();
            int studentCount = 0;
            if (studentResponse != null && studentResponse.IsSuccess)
            {
                var students = JsonConvert.DeserializeObject<List<StudentDto>>(Convert.ToString(studentResponse.Result));
                studentCount = students?.Count ?? 0;
            }

            // Fetch professor count
            ResponseDto? professorResponse = await _adminService.GetProfessorsAsync();
            int professorCount = 0;
            if (professorResponse != null && professorResponse.IsSuccess)
            {
                var professors = JsonConvert.DeserializeObject<List<ProfessorDto>>(Convert.ToString(professorResponse.Result));
                professorCount = professors?.Count ?? 0;
            }

            // Fetch courses
            var courseResponse = await _courseService.GetAllCoursesAsync();
            int courseCount = 0;
            if (courseResponse != null && courseResponse.IsSuccess)
            {
                var courses = JsonConvert.DeserializeObject<List<CourseDto>>(Convert.ToString(courseResponse.Result));
                courseCount = courses?.Count ?? 0;
            }

            // Fetch enrollments
            var enrollmentResponse = await _enrollmentService.GetAllEnrollmentsAsync();
            int activeEnrollmentCount = 0;
            if (enrollmentResponse != null && enrollmentResponse.IsSuccess)
            {
                var enrollments = JsonConvert.DeserializeObject<List<CourseRegistrationDto>>(Convert.ToString(enrollmentResponse.Result));
                activeEnrollmentCount = enrollments?.Count(e => e.RegistrationStatus == "Active") ?? 0;
            }

            // Create view model
            var metrics = new SystemMetricsViewModel
            {
                StudentCount = studentCount,
                ProfessorCount = professorCount,
                CourseCount = courseCount,
                ActiveEnrollmentCount = activeEnrollmentCount
            };

            return View(metrics);
        }


        [HttpGet]
        public async Task<IActionResult> EnrollmentManagement()
        {
            var enrollmentResponse = await _enrollmentService.GetAllEnrollmentsAsync();
            List<CourseRegistrationDto> enrollments = new();

            if (enrollmentResponse != null && enrollmentResponse.IsSuccess)
            {
                enrollments = JsonConvert.DeserializeObject<List<CourseRegistrationDto>>(Convert.ToString(enrollmentResponse.Result));
            }

            // Get course details for all enrollments
            Dictionary<string, string> courseTitles = new();
            foreach (var enrollment in enrollments)
            {
                if (!courseTitles.ContainsKey(enrollment.CourseCode))
                {
                    var courseResponse = await _courseService.GetCourseByCodeAsync(enrollment.CourseCode);
                    if (courseResponse != null && courseResponse.IsSuccess)
                    {
                        var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(courseResponse.Result));
                        courseTitles[enrollment.CourseCode] = course.Title;
                    }
                    else
                    {
                        courseTitles[enrollment.CourseCode] = enrollment.CourseCode;
                    }
                }
            }

            ViewBag.CourseTitles = courseTitles;

            // Get all students for dropdown
            var studentResponse = await _adminService.GetStudentsAsync();
            List<StudentDto> students = new();
            if (studentResponse != null && studentResponse.IsSuccess)
            {
                students = JsonConvert.DeserializeObject<List<StudentDto>>(Convert.ToString(studentResponse.Result));
            }
            ViewBag.Students = students;

            // Get all courses for dropdown
            var courseListResponse = await _courseService.GetAllCoursesAsync();
            List<CourseDto> courses = new();
            if (courseListResponse != null && courseListResponse.IsSuccess)
            {
                courses = JsonConvert.DeserializeObject<List<CourseDto>>(Convert.ToString(courseListResponse.Result));
            }
            ViewBag.Courses = courses;

            return View(enrollments);
        }

        [HttpPost]
        public async Task<IActionResult> EnrollStudent(string courseCode, string studentUniversityId)
        {
            if (string.IsNullOrEmpty(courseCode) || string.IsNullOrEmpty(studentUniversityId))
            {
                TempData["error"] = "Course code and student ID are required";
                return RedirectToAction("EnrollmentManagement");
            }

            // Check if student is already enrolled
            var checkEnrollmentResponse = await _enrollmentService.IsStudentEnrolledAsync(courseCode, studentUniversityId);
            if (checkEnrollmentResponse != null && checkEnrollmentResponse.IsSuccess)
            {
                bool isAlreadyEnrolled = Convert.ToBoolean(checkEnrollmentResponse.Result);
                if (isAlreadyEnrolled)
                {
                    TempData["error"] = "Student is already enrolled in this course";
                    return RedirectToAction("EnrollmentManagement");
                }
            }

            // Enroll the student
            var enrollResponse = await _enrollmentService.EnrollStudentAsync(courseCode, studentUniversityId);
            if (enrollResponse != null && enrollResponse.IsSuccess)
            {
                TempData["success"] = "Student successfully enrolled in the course";
            }
            else
            {
                TempData["error"] = enrollResponse?.Message ?? "Failed to enroll student";
            }

            return RedirectToAction("EnrollmentManagement");
        }

        [HttpPost]
        public async Task<IActionResult> UnenrollStudent(int enrollmentId)
        {
            if (enrollmentId <= 0)
            {
                TempData["error"] = "Invalid enrollment ID";
                return RedirectToAction("EnrollmentManagement");
            }

            var response = await _enrollmentService.DropCourseAsync(enrollmentId);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Student successfully unenrolled from course";
            }
            else
            {
                TempData["error"] = response?.Message ?? "Failed to unenroll student";
            }

            return RedirectToAction("EnrollmentManagement");
        }
        [HttpGet]
        public async Task<IActionResult> GradeOverride()
        {
            // Get all quiz attempts
            var attemptResponse = await _studentQuizAttemptService.GetAllQuizAttemptsAsync();
            List<StudentQuizAttemptDto> attempts = new();

            if (attemptResponse != null && attemptResponse.IsSuccess)
            {
                attempts = JsonConvert.DeserializeObject<List<StudentQuizAttemptDto>>(Convert.ToString(attemptResponse.Result));
            }

            // Get quiz titles
            Dictionary<string, string> quizTitles = new();
            foreach (var attempt in attempts)
            {
                if (!quizTitles.ContainsKey(attempt.QuizCode))
                {
                    var quizResponse = await _quizService.GetQuizByCodeAsync(attempt.QuizCode);
                    if (quizResponse != null && quizResponse.IsSuccess)
                    {
                        var quiz = JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(quizResponse.Result));
                        quizTitles[attempt.QuizCode] = quiz.Title;
                    }
                    else
                    {
                        quizTitles[attempt.QuizCode] = attempt.QuizCode;
                    }
                }
            }

            ViewBag.QuizTitles = quizTitles;

            return View(attempts);
        }

        [HttpGet]
        public async Task<IActionResult> GradeAttempt(string attemptCode)
        {
            if (string.IsNullOrEmpty(attemptCode))
            {
                TempData["error"] = "Invalid attempt code";
                return RedirectToAction("GradeOverride");
            }

            // Get attempt information
            var attemptResponse = await _studentQuizAttemptService.GetAttemptByCodeAsync(attemptCode);
            if (attemptResponse == null || !attemptResponse.IsSuccess)
            {
                TempData["error"] = "Quiz attempt not found";
                return RedirectToAction("GradeOverride");
            }

            var attempt = JsonConvert.DeserializeObject<StudentQuizAttemptDto>(Convert.ToString(attemptResponse.Result));

            // Get quiz information
            var quizResponse = await _quizService.GetQuizByCodeAsync(attempt.QuizCode);
            if (quizResponse == null || !quizResponse.IsSuccess)
            {
                TempData["error"] = "Quiz not found";
                return RedirectToAction("GradeOverride");
            }

            var quiz = JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(quizResponse.Result));

            // Get student answers for this attempt
            var answersResponse = await _studentAnswerService.GetAnswersByAttemptCodeAsync(attemptCode);
            List<StudentAnswerDto> answers = new();

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

        [HttpPost]
        public async Task<IActionResult> OverrideGrade(StudentAnswerGradeDto gradeDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Invalid grade data";
                return RedirectToAction("GradeOverride");
            }

            // Get the answer to find the attempt
            var answerResponse = await _studentAnswerService.GetAnswerByIdAsync(gradeDto.Id);
            if (answerResponse == null || !answerResponse.IsSuccess)
            {
                TempData["error"] = "Answer not found";
                return RedirectToAction("GradeOverride");
            }

            var answer = JsonConvert.DeserializeObject<StudentAnswerDto>(Convert.ToString(answerResponse.Result));
            string attemptCode = answer.AttemptCode;

            // Grade the answer
            var response = await _studentAnswerService.GradeStudentAnswerAsync(gradeDto);

            if (response != null && response.IsSuccess)
            {
                // Recalculate the score for the attempt
                await _studentQuizAttemptService.CalculateAndUpdateScoreAsync(attemptCode);

                TempData["success"] = "Grade overridden successfully";
            }
            else
            {
                TempData["error"] = response?.Message ?? "Failed to override grade";
            }

            return RedirectToAction("GradeAttempt", new { attemptCode });
        }
        [HttpGet]
        public async Task<IActionResult> PerformanceReports()
        {
            // Get all courses
            var courseResponse = await _courseService.GetAllCoursesAsync();
            List<CourseDto> courses = new();

            if (courseResponse != null && courseResponse.IsSuccess)
            {
                courses = JsonConvert.DeserializeObject<List<CourseDto>>(Convert.ToString(courseResponse.Result));
            }

            CourseSummaryViewModel model = new();

            // For each course, get enrollment and quiz data
            foreach (var course in courses)
            {
                // Get enrollments for the course
                var enrollmentsResponse = await _enrollmentService.GetCourseEnrollmentsAsync(course.CourseCode);
                int totalEnrolledStudents = 0;

                if (enrollmentsResponse != null && enrollmentsResponse.IsSuccess)
                {
                    var enrollments = JsonConvert.DeserializeObject<List<CourseRegistrationDto>>(Convert.ToString(enrollmentsResponse.Result));
                    totalEnrolledStudents = enrollments?.Count(e => e.RegistrationStatus == "Active") ?? 0;
                }

                // Get quizzes for the course
                var quizzesResponse = await _quizService.GetQuizzesByCourseCodeAsync(course.CourseCode);
                if (quizzesResponse != null && quizzesResponse.IsSuccess)
                {
                    var quizzes = JsonConvert.DeserializeObject<List<QuizDto>>(Convert.ToString(quizzesResponse.Result));

                    foreach (var quiz in quizzes)
                    {
                        // Get attempts for the quiz
                        var attemptsResponse = await _studentQuizAttemptService.GetAttemptsByQuizCodeAsync(quiz.QuizCode);
                        List<StudentQuizAttemptDto> attempts = new();

                        if (attemptsResponse != null && attemptsResponse.IsSuccess)
                        {
                            attempts = JsonConvert.DeserializeObject<List<StudentQuizAttemptDto>>(Convert.ToString(attemptsResponse.Result));
                        }

                        // Create quiz performance data
                        var quizPerformance = new QuizPerformanceViewModel
                        {
                            QuizCode = quiz.QuizCode,
                            QuizTitle = quiz.Title,
                            CourseCode = course.CourseCode,
                            CourseTitle = course.Title,
                            TotalEnrolledStudents = totalEnrolledStudents,
                            StudentsAttempted = attempts?.Count ?? 0,
                            AverageScore = attempts != null && attempts.Any() ?
                                Math.Round(attempts.Average(a => a.Score), 2) : 0
                        };

                        model.QuizPerformances.Add(quizPerformance);
                    }
                }
            }

            return View(model);
        }

    }
}
