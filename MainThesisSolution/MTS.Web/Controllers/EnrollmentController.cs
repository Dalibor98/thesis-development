using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTS.Web.Models;
using MTS.Web.Models.Curriculum.Course;
using MTS.Web.Models.User.Student;
using MTS.Web.Service;
using MTS.Web.Service.IService;
using MTS.Web.Utility;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MTS.Web.Controllers
{
    [Authorize]
    public class EnrollmentController : Controller
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly ICourseService _courseService;
        private readonly IStudentService _studentService;

        public EnrollmentController(IEnrollmentService enrollmentService, ICourseService courseService, IStudentService studentService)
        {
            _enrollmentService = enrollmentService;
            _courseService = courseService;
            _studentService = studentService;
        }

        [Authorize(Roles = SD.RoleSidekick)]
        public async Task<IActionResult> MyCourses()
        {
            var studentId = User.FindFirstValue("UniversityId");

            var response = await _enrollmentService.GetStudentEnrollmentsAsync(studentId);

            if (response != null && response.IsSuccess)
            {
                var enrollments = JsonConvert.DeserializeObject<List<CourseRegistrationDto>>(Convert.ToString(response.Result));

                // Get course details for each enrollment
                List<CourseDto> courses = new List<CourseDto>();

                foreach (var enrollment in enrollments.Where(e => e.RegistrationStatus == "Active"))
                {
                    var courseResponse = await _courseService.GetCourseByCodeAsync(enrollment.CourseCode);
                    if (courseResponse != null && courseResponse.IsSuccess)
                    {
                        var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(courseResponse.Result));
                        courses.Add(course);
                    }
                }

                return View(courses);
            }

            return View(new List<CourseDto>());
        }

        [Authorize(Roles = SD.RoleSidekick)]
        public async Task<IActionResult> Enroll(string courseCode)
        {
            var studentId = User.FindFirstValue("UniversityId");

            // Check if already enrolled
            var checkResponse = await _enrollmentService.IsStudentEnrolledAsync(courseCode, studentId);
            if (checkResponse != null && checkResponse.IsSuccess)
            {
                // Get enrollment details to check status
                var enrollmentResponse = await _enrollmentService.GetCourseEnrollmentsAsync(courseCode);
                if (enrollmentResponse != null && enrollmentResponse.IsSuccess)
                {
                    var enrollments = JsonConvert.DeserializeObject<List<CourseRegistrationDto>>(Convert.ToString(enrollmentResponse.Result));
                    var existingEnrollment = enrollments?.FirstOrDefault(e => e.StudentCode == studentId);

                    if (existingEnrollment != null)
                    {
                        if (existingEnrollment.RegistrationStatus == "Active")
                        {
                            TempData["info"] = "You are already enrolled in this course";
                            return RedirectToAction("Details", "Course", new { courseCode });
                        }
                        else if (existingEnrollment.RegistrationStatus == "Dropped")
                        {
                            TempData["error"] = "You have been removed from this course and cannot re-enroll.";
                            return RedirectToAction("Details", "Course", new { courseCode });
                        }
                    }
                }
            }

            // Enroll the student
            var response = await _enrollmentService.EnrollStudentAsync(courseCode, studentId);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Successfully enrolled in the course";
            }
            else
            {
                TempData["error"] = response?.Message ?? "Failed to enroll in the course";
            }

            return RedirectToAction("Details", "Course", new { courseCode });
        }

        [Authorize(Roles = SD.RoleSidekick)]
        public async Task<IActionResult> Drop(int enrollmentId, string courseCode)
        {
            var response = await _enrollmentService.DropCourseAsync(enrollmentId);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Successfully dropped the course";
            }
            else
            {
                TempData["error"] = response?.Message ?? "Failed to drop the course";
            }

            return RedirectToAction("MyCourses");
        }

        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> ProfessorEnrollments()
        {
            // Get the current professor's ID
            var professorId = User.FindFirstValue("UniversityId");
            if (string.IsNullOrEmpty(professorId))
            {
                TempData["error"] = "Unable to identify the current professor.";
                return RedirectToAction("Index", "Home");
            }

            // Get the professor's courses
            var coursesResponse = await _courseService.GetProfessorCoursesAsync(professorId);
            if (coursesResponse == null || !coursesResponse.IsSuccess)
            {
                TempData["error"] = "Failed to retrieve professor's courses.";
                return View(new List<CourseRegistrationDto>());
            }

            var professorCourses = JsonConvert.DeserializeObject<List<CourseDto>>(Convert.ToString(coursesResponse.Result));
            if (professorCourses == null || !professorCourses.Any())
            {
                ViewBag.Message = "You don't have any courses. Create a course first to see enrollments.";
                return View(new List<CourseRegistrationDto>());
            }

            // Store course titles for display
            Dictionary<string, string> courseTitles = professorCourses.ToDictionary(c => c.CourseCode, c => c.Title);
            ViewBag.CourseTitles = courseTitles;
            ViewBag.Courses = professorCourses;

            // Get all enrollments for the professor's courses
            List<CourseRegistrationDto> allEnrollments = new List<CourseRegistrationDto>();
            foreach (var course in professorCourses)
            {
                var enrollmentsResponse = await _enrollmentService.GetCourseEnrollmentsAsync(course.CourseCode);
                if (enrollmentsResponse != null && enrollmentsResponse.IsSuccess)
                {
                    var courseEnrollments = JsonConvert.DeserializeObject<List<CourseRegistrationDto>>(Convert.ToString(enrollmentsResponse.Result));
                    if (courseEnrollments != null && courseEnrollments.Any())
                    {
                        allEnrollments.AddRange(courseEnrollments);
                    }
                }
            }

            // Get student details for display
            HashSet<string> studentIds = new HashSet<string>(allEnrollments.Select(e => e.StudentCode));
            Dictionary<string, string> studentNames = new Dictionary<string, string>();

            if (studentIds.Any())
            {
                var studentsResponse = await _studentService.GetAllStudentsAsync();
                if (studentsResponse != null && studentsResponse.IsSuccess)
                {
                    var allStudents = JsonConvert.DeserializeObject<List<StudentDto>>(Convert.ToString(studentsResponse.Result));
                    if (allStudents != null)
                    {
                        foreach (var student in allStudents.Where(s => studentIds.Contains(s.UniversityId)))
                        {
                            studentNames[student.UniversityId] = student.Name;
                        }
                    }
                }
            }

            ViewBag.StudentNames = studentNames;

            return View(allEnrollments);
        }

        [HttpPost]
        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> UnenrollStudent(int enrollmentId, string returnUrl = null)
        {
            if (enrollmentId <= 0)
            {
                TempData["error"] = "Invalid enrollment ID";
                return RedirectToAction("ProfessorEnrollments");
            }

            // Get the current professor's ID
            var professorId = User.FindFirstValue("UniversityId");

            // Get all courses for this professor
            var coursesResponse = await _courseService.GetProfessorCoursesAsync(professorId);
            if (coursesResponse == null || !coursesResponse.IsSuccess)
            {
                TempData["error"] = "Failed to retrieve professor's courses";
                return RedirectToAction("ProfessorEnrollments");
            }

            var professorCourses = JsonConvert.DeserializeObject<List<CourseDto>>(Convert.ToString(coursesResponse.Result));
            var professorCourseCodes = professorCourses?.Select(c => c.CourseCode).ToList() ?? new List<string>();

            // Get all enrollments for the professor's courses to verify ownership
            bool foundEnrollment = false;
            CourseRegistrationDto targetEnrollment = null;

            foreach (var courseCode in professorCourseCodes)
            {
                var enrollmentsResponse = await _enrollmentService.GetCourseEnrollmentsAsync(courseCode);
                if (enrollmentsResponse != null && enrollmentsResponse.IsSuccess)
                {
                    var courseEnrollments = JsonConvert.DeserializeObject<List<CourseRegistrationDto>>(Convert.ToString(enrollmentsResponse.Result));
                    if (courseEnrollments != null)
                    {
                        var enrollment = courseEnrollments.FirstOrDefault(e => e.Id == enrollmentId);
                        if (enrollment != null)
                        {
                            foundEnrollment = true;
                            targetEnrollment = enrollment;
                            break;
                        }
                    }
                }
            }

            // If enrollment wasn't found in any of the professor's courses
            if (!foundEnrollment || targetEnrollment == null)
            {
                TempData["error"] = "You do not have permission to manage this enrollment";
                return RedirectToAction("ProfessorEnrollments");
            }

            // Now we've verified the professor owns this course, proceed with unenrollment
            var response = await _enrollmentService.DropCourseAsync(enrollmentId);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = $"Student {targetEnrollment.StudentCode} has been removed from the course. All their course data has been deleted.";
            }
            else
            {
                TempData["error"] = response?.Message ?? "Failed to remove student from course";
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("ProfessorEnrollments");
        }

    }
}