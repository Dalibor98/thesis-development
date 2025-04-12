using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTS.Web.Models;
using MTS.Web.Models.Curriculum.Assignment;
using MTS.Web.Models.Curriculum.Course;
using MTS.Web.Service.IService;
using MTS.Web.Utility;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MTS.Web.Controllers
{
    [Authorize(Roles = SD.RoleLeader)]
    public class AssignmentController : Controller
    {
        private readonly IAssignmentService _assignmentService;
        private readonly ICourseService _courseService;

        public AssignmentController(IAssignmentService assignmentService, ICourseService courseService)
        {
            _assignmentService = assignmentService;
            _courseService = courseService;
        }

        public async Task<IActionResult> View(string assignmentCode)
        {
            ResponseDto? response = await _assignmentService.GetAssignmentByCodeAsync(assignmentCode);

            if (response != null && response.IsSuccess)
            {
                AssignmentDto? assignment = JsonConvert.DeserializeObject<AssignmentDto>(Convert.ToString(response.Result));

                // Check if the user is a professor
                if (User.IsInRole(SD.RoleLeader))
                {
                    // Get all submissions for this assignment
                    ResponseDto? submissionsResponse = await _assignmentService.GetSubmissionsByAssignmentCodeAsync(assignmentCode);
                    if (submissionsResponse != null && submissionsResponse.IsSuccess)
                    {
                        ViewBag.Submissions = JsonConvert.DeserializeObject<List<StudentAssignmentAttemptDto>>(Convert.ToString(submissionsResponse.Result));
                    }
                    else
                    {
                        ViewBag.Submissions = new List<StudentAssignmentAttemptDto>();
                    }

                    // Get course info to verify ownership
                    var courseResponse = await _courseService.GetCourseByCodeAsync(assignment.CourseCode);
                    if (courseResponse != null && courseResponse.IsSuccess)
                    {
                        var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(courseResponse.Result));
                        ViewBag.Course = course;
                    }

                    // Return the professor view
                    return View("ProfessorView", assignment);
                }
                else if (User.IsInRole(SD.RoleSidekick))
                {
                    // For students, check if they have a submission
                    string studentId = User.FindFirstValue("UniversityId");
                    ResponseDto? submissionResponse = await _assignmentService.GetStudentSubmissionAsync(assignmentCode, studentId);

                    if (submissionResponse != null && submissionResponse.IsSuccess)
                    {
                        ViewBag.Submission = JsonConvert.DeserializeObject<StudentAssignmentAttemptDto>(Convert.ToString(submissionResponse.Result));
                    }

                    // Return the student view
                    return View(assignment);
                }
                else
                {
                    // For unauthenticated users, redirect to login
                    return RedirectToAction("Login", "Auth");
                }
            }

            TempData["error"] = response?.Message ?? "Assignment not found";
            return RedirectToAction("Index", "Home");
        }

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
                TempData["error"] = "You are not authorized to add assignments to this course";
                return RedirectToAction("Details", "Course", new { courseCode = courseCode });
            }

            var assignmentCreateDto = new AssignmentCreateDto
            {
                WeekCode = weekCode,
                CourseCode = courseCode,
                DueDate = DateTime.Now.AddDays(7) // Default due date 1 week from now
            };

            return View(assignmentCreateDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AssignmentCreateDto assignmentDto)
        {
            if (ModelState.IsValid)
            {
                var response = await _assignmentService.CreateAssignmentAsync(assignmentDto);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Assignment created successfully";
                    return RedirectToAction("Details", "Course", new { courseCode = assignmentDto.CourseCode });
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }

            return View(assignmentDto);
        }

        public async Task<IActionResult> Edit(string assignmentCode)
        {
            var response = await _assignmentService.GetAssignmentByCodeAsync(assignmentCode);

            if (response != null && response.IsSuccess)
            {
                var assignment = JsonConvert.DeserializeObject<AssignmentDto>(Convert.ToString(response.Result));

                // Get course info to verify ownership
                var courseResponse = await _courseService.GetCourseByCodeAsync(assignment.CourseCode);
                if (courseResponse != null && courseResponse.IsSuccess)
                {
                    var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(courseResponse.Result));

                    // Verify the current user is the professor for this course
                    var userUniversityId = User.FindFirstValue("UniversityId");
                    if (course.ProfessorUniversityId != userUniversityId)
                    {
                        TempData["error"] = "You are not authorized to edit this assignment";
                        return RedirectToAction("Details", "Course", new { courseCode = assignment.CourseCode });
                    }

                    var assignmentEditDto = new AssignmentUpdateDto
                    {
                        CourseCode = assignment.CourseCode,
                        WeekCode = assignment.WeekCode,
                        AssignmentCode = assignment.AssignmentCode,
                        Title = assignment.Title,
                        Description = assignment.Description,
                        MaxPoints = assignment.MaxPoints,
                        MinPoints = assignment.MinPoints,
                        DueDate = assignment.DueDate
                    };

                    return View(assignmentEditDto);
                }
            }

            TempData["error"] = response?.Message ?? "Assignment not found";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AssignmentUpdateDto assignmentDto)
        {
            if (ModelState.IsValid)
            {
                // Verify ownership
                var courseResponse = await _courseService.GetCourseByCodeAsync(assignmentDto.CourseCode);
                if (courseResponse != null && courseResponse.IsSuccess)
                {
                    var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(courseResponse.Result));

                    // Verify the current user is the professor for this course
                    var userUniversityId = User.FindFirstValue("UniversityId");
                    if (course.ProfessorUniversityId != userUniversityId)
                    {
                        TempData["error"] = "You are not authorized to edit this assignment";
                        return RedirectToAction("Details", "Course", new { courseCode = assignmentDto.CourseCode });
                    }

                    var response = await _assignmentService.UpdateAssignmentAsync(assignmentDto);

                    if (response != null && response.IsSuccess)
                    {
                        TempData["success"] = "Assignment updated successfully";
                        return RedirectToAction("Details", "Course", new { courseCode = assignmentDto.CourseCode });
                    }
                    else
                    {
                        TempData["error"] = response?.Message;
                    }
                }
            }

            return View(assignmentDto);
        }

        public async Task<IActionResult> Delete(string assignmentCode)
        {
            var response = await _assignmentService.GetAssignmentByCodeAsync(assignmentCode);

            if (response != null && response.IsSuccess)
            {
                var assignment = JsonConvert.DeserializeObject<AssignmentDto>(Convert.ToString(response.Result));

                // Verify ownership
                var courseResponse = await _courseService.GetCourseByCodeAsync(assignment.CourseCode);
                if (courseResponse != null && courseResponse.IsSuccess)
                {
                    var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(courseResponse.Result));

                    // Verify the current user is the professor for this course
                    var userUniversityId = User.FindFirstValue("UniversityId");
                    if (course.ProfessorUniversityId != userUniversityId)
                    {
                        TempData["error"] = "You are not authorized to delete this assignment";
                        return RedirectToAction("Details", "Course", new { courseCode = assignment.CourseCode });
                    }

                    return View(assignment);
                }
            }

            TempData["error"] = response?.Message ?? "Assignment not found";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string assignmentCode)
        {
            var assignmentResponse = await _assignmentService.GetAssignmentByCodeAsync(assignmentCode);
            if (assignmentResponse != null && assignmentResponse.IsSuccess)
            {
                var assignment = JsonConvert.DeserializeObject<AssignmentDto>(Convert.ToString(assignmentResponse.Result));

                var response = await _assignmentService.DeleteAssignmentAsync(assignmentCode);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Assignment deleted successfully";
                    return RedirectToAction("Details", "Course", new { courseCode = assignment.CourseCode });
                }
                else
                {
                    TempData["error"] = response?.Message;
                }

                return RedirectToAction("Details", "Course", new { courseCode = assignment.CourseCode });
            }

            TempData["error"] = "Error retrieving assignment details";
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [Authorize(Roles = SD.RoleSidekick)]
        public async Task<IActionResult> Submit(StudentAssignmentAttemptCreateDto submissionDto)
        {
            if (!ModelState.IsValid)
            {
                return View(submissionDto);
            }

            // Add current student's ID
            submissionDto.StudentUniversityId = User.FindFirstValue("UniversityId");

            ResponseDto? response = await _assignmentService.SubmitAssignmentAsync(submissionDto);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Assignment submitted successfully";
                return RedirectToAction("View", new { assignmentCode = submissionDto.AssignmentCode });
            }

            TempData["error"] = response?.Message;

            // Redirect back to the assignment view
            return RedirectToAction("View", new { assignmentCode = submissionDto.AssignmentCode });
        }

        [HttpPost]
        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> Grade(StudentAssignmentGradeDto gradeDto)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("View", new { assignmentCode = gradeDto.AssignmentCode });
            }

            ResponseDto? response = await _assignmentService.GradeAssignmentAsync(gradeDto);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Assignment graded successfully";
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return RedirectToAction("View", new { assignmentCode = gradeDto.AssignmentCode });
        }
    }
}