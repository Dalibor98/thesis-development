using Azure;
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
        protected ResponseDto _response;

        public StudentAssignmentAttemptsController(IStudentAssignmentAttemptRepository attemptRepository)
        {
            _attemptRepository = attemptRepository;
            _response = new ResponseDto();
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAttempt(int id, [FromBody] StudentAssignmentAttemptUpdateDto attemptDto)
        {
            if (id != attemptDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                var result = await _attemptRepository.UpdateAttemptAsync(attemptDto);
                if (result == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Submission not found";
                    return NotFound(_response);
                }

                _response.Result = result;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

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