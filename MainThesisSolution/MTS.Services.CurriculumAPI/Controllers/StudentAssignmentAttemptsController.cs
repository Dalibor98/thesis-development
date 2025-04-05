using Microsoft.AspNetCore.Mvc;
using MTS.Services.CurriculumAPI.Models.DTO;
using MTS.Services.CurriculumAPI.Repository.IRepository;

namespace MTS.Services.CurriculumAPI.Controllers
{
    [Route("api/assignmentAttempts")]
    [ApiController]
    public class StudentAssignmentAttemptsController : ControllerBase
    {
        private readonly IStudentAssignmentAttemptRepository _attemptRepository;

        public StudentAssignmentAttemptsController(IStudentAssignmentAttemptRepository attemptRepository)
        {
            _attemptRepository = attemptRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAttempt(StudentAssignmentAttemptCreateDto attemptDto)
        {
            try
            {
                var result = await _attemptRepository.CreateAttemptAsync(attemptDto);
                return Created($"/api/StudentAssignmentAttempts/{result.Id}", result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
        /*

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAttempt(int id, StudentAssignmentAttemptUpdateDto attemptDto)
        {
            if (id != attemptDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            var result = await _attemptRepository.UpdateAttemptAsync(attemptDto);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        */

        [HttpGet("student/{studentUniversityId}")]
        public async Task<IActionResult> GetAttemptsByStudent(string studentUniversityId)
        {
            var attempts = await _attemptRepository.GetAttemptsByStudentIdAsync(studentUniversityId);
            return Ok(attempts);
        }

        [HttpGet("assignment/{assignmentCode}")]
        public async Task<IActionResult> GetAttemptsByAssignment(string assignmentCode)
        {
            var attempts = await _attemptRepository.GetAttemptsByAssignmentCodeAsync(assignmentCode);
            return Ok(attempts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAttemptById(int id)
        {
            var attempt = await _attemptRepository.GetAttemptByIdAsync(id);
            if (attempt == null)
            {
                return NotFound();
            }
            return Ok(attempt);
        }

        [HttpGet("student/{studentUniversityId}/assignment/{assignmentCode}")]
        public async Task<IActionResult> GetAttemptByStudentAndAssignment(
            string studentUniversityId, string assignmentCode)
        {
            var attempt = await _attemptRepository.GetAttemptByStudentAndAssignmentAsync(
                studentUniversityId, assignmentCode);

            if (attempt == null)
            {
                return NotFound();
            }

            return Ok(attempt);
        }
    }
}