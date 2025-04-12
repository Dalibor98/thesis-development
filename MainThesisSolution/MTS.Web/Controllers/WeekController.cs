using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTS.Web.Models.Curriculum.Course;
using MTS.Web.Models.Curriculum.Week;
using MTS.Web.Service.IService;
using MTS.Web.Utility;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MTS.Web.Controllers
{
    [Authorize(Roles = SD.RoleLeader)]
    public class WeekController : Controller
    {
        private readonly IWeekService _weekService;
        private readonly ICourseService _courseService;

        public WeekController(IWeekService weekService, ICourseService courseService)
        {
            _weekService = weekService;
            _courseService = courseService;
        }

        public async Task<IActionResult> Create(string courseCode)
        {
            // Verify the user is the professor for this course
            var courseResponse = await _courseService.GetCourseByCodeAsync(courseCode);
            if (courseResponse != null && courseResponse.IsSuccess)
            {
                var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(courseResponse.Result));
                var userUniversityId = User.FindFirstValue("UniversityId");

                if (course.ProfessorUniversityId != userUniversityId)
                {
                    TempData["error"] = "You are not authorized to add weeks to this course";
                    return RedirectToAction("Details", "Course", new { courseCode });
                }

                var weekCreateDto = new WeekCreateDto
                {
                    CourseCode = courseCode
                };

                return View(weekCreateDto);
            }

            TempData["error"] = courseResponse?.Message ?? "Course not found";
            return RedirectToAction("Index", "Course");
        }

        [HttpPost]
        public async Task<IActionResult> Create(WeekCreateDto weekDto)
        {
            if (ModelState.IsValid)
            {
                // Verify the user is the professor for this course
                var courseResponse = await _courseService.GetCourseByCodeAsync(weekDto.CourseCode);
                if (courseResponse != null && courseResponse.IsSuccess)
                {
                    var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(courseResponse.Result));
                    var userUniversityId = User.FindFirstValue("UniversityId");

                    if (course.ProfessorUniversityId != userUniversityId)
                    {
                        TempData["error"] = "You are not authorized to add weeks to this course";
                        return RedirectToAction("Details", "Course", new { courseCode = weekDto.CourseCode });
                    }

                    var response = await _weekService.CreateWeekAsync(weekDto);

                    if (response != null && response.IsSuccess)
                    {
                        TempData["success"] = "Week created successfully";
                        return RedirectToAction("Details", "Course", new { courseCode = weekDto.CourseCode });
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
            }

            return View(weekDto);
        }
    }
}