using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTS.Web.Models;
using MTS.Web.Service.IService;
using MTS.Web.Utility;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace MTS.Web.Controllers
{
    public class AssignmentController : Controller
    {
        private readonly IAssignmentService _assignmentService;
        private readonly ITokenProvider _tokenProvider;

        public AssignmentController(IAssignmentService assignmentService, ITokenProvider tokenProvider)
        {
            _assignmentService = assignmentService;
            _tokenProvider = tokenProvider;
        }

        // Student View - View a specific assignment
        [Authorize(Roles = SD.RoleSidekick)]
        public async Task<IActionResult> View(string assignmentCode)
        {
            ResponseDto? response = await _assignmentService.GetAssignmentByCodeAsync(assignmentCode);

            if (response != null && response.IsSuccess)
            {
                AssignmentDto? assignment = JsonConvert.DeserializeObject<AssignmentDto>(Convert.ToString(response.Result));

                // Get current submission status if any
                string studentId = GetCurrentUserUniversityId();
                ResponseDto? submissionResponse = await _assignmentService.GetStudentSubmissionAsync(assignmentCode, studentId);

                if (submissionResponse != null && submissionResponse.IsSuccess)
                {
                    ViewBag.Submission = JsonConvert.DeserializeObject<StudentAssignmentAttemptDto>(Convert.ToString(submissionResponse.Result));
                }

                return View(assignment);
            }

            TempData["error"] = response?.Message;
            return RedirectToAction("Index", "Home");
        }

        // Student View - Submit an assignment
        [HttpPost]
        [Authorize(Roles = SD.RoleSidekick)]
        public async Task<IActionResult> Submit(StudentAssignmentAttemptCreateDto submissionDto)
        {
            if (!ModelState.IsValid)
            {
                return View(submissionDto);
            }

            // Add current student's ID
            submissionDto.StudentUniversityId = GetCurrentUserUniversityId();

            ResponseDto? response = await _assignmentService.SubmitAssignmentAsync(submissionDto);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Assignment submitted successfully";
                return RedirectToAction("View", new { assignmentCode = submissionDto.AssignmentCode });
            }

            TempData["error"] = response?.Message;
            return View("View", submissionDto);
        }

        // Professor View - View all submissions for an assignment
        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> Submissions(string assignmentCode)
        {
            // Get the assignment details
            ResponseDto? assignmentResponse = await _assignmentService.GetAssignmentByCodeAsync(assignmentCode);

            if (assignmentResponse == null || !assignmentResponse.IsSuccess)
            {
                TempData["error"] = assignmentResponse?.Message ?? "Could not retrieve assignment details";
                return RedirectToAction("Index", "Home");
            }

            AssignmentDto? assignment = JsonConvert.DeserializeObject<AssignmentDto>(Convert.ToString(assignmentResponse.Result));
            ViewBag.Assignment = assignment;

            // Get all submissions
            ResponseDto? submissionsResponse = await _assignmentService.GetSubmissionsByAssignmentCodeAsync(assignmentCode);

            if (submissionsResponse != null && submissionsResponse.IsSuccess)
            {
                List<StudentAssignmentAttemptDto>? submissions =
                    JsonConvert.DeserializeObject<List<StudentAssignmentAttemptDto>>(Convert.ToString(submissionsResponse.Result));

                return View(submissions);
            }

            TempData["error"] = submissionsResponse?.Message;
            return View(new List<StudentAssignmentAttemptDto>());
        }

        // Professor View - Grade a submission
        [HttpPost]
        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> Grade(StudentAssignmentGradeDto gradeDto)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Submissions", new { assignmentCode = gradeDto.AssignmentCode });
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

            return RedirectToAction("Submissions", new { assignmentCode = gradeDto.AssignmentCode });
        }

        // Helper method to get current user's university ID from claims
        private string GetCurrentUserUniversityId()
        {
            // This is a simplification - in your app, you'll need to retrieve this from user claims
            // or from your user database based on the current user's identity
            var token = _tokenProvider.GetToken();
            if (string.IsNullOrEmpty(token))
                return string.Empty;

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            // Assuming UniversityId is stored in a claim
            var universityIdClaim = jwt.Claims.FirstOrDefault(c => c.Type == "UniversityId");
            return universityIdClaim?.Value ?? string.Empty;
        }
    }
}