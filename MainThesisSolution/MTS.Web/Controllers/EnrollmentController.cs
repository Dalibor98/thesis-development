using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTS.Web.Models;
using MTS.Web.Models.Curriculum.Course;
using MTS.Web.Service.IService;
using MTS.Web.Utility;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MTS.Web.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly ICourseService _courseService;

        public EnrollmentController(IEnrollmentService enrollmentService, ICourseService courseService)
        {
            _enrollmentService = enrollmentService;
            _courseService = courseService;
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
                bool isEnrolled = JsonConvert.DeserializeObject<bool>(Convert.ToString(checkResponse.Result));
                if (isEnrolled)
                {
                    TempData["info"] = "You are already enrolled in this course";
                    return RedirectToAction("Details", "Course", new { courseCode });
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
    }
}