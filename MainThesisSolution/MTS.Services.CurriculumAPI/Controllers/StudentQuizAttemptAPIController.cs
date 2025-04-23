using Microsoft.AspNetCore.Mvc;
using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO;
using MTS.Services.CurriculumAPI.Models.DTO.QuizDto;
using MTS.Services.CurriculumAPI.Repository.IRepository;

namespace MTS.Services.CurriculumAPI.Controllers
{
    [Route("api/quizattempts")]
    [ApiController]
    public class StudentQuizAttemptAPIController : ControllerBase
    {
        private readonly IStudentQuizAttemptRepository _attemptRepository;
        protected ResponseDto _response;

        public StudentQuizAttemptAPIController(IStudentQuizAttemptRepository attemptRepository)
        {
            _attemptRepository = attemptRepository ?? throw new ArgumentNullException(nameof(attemptRepository));
            _response = new ResponseDto();
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto>> GetAllAttempts()
        {
            try
            {
                var attempts = await _attemptRepository.GetAllAttemptsAsync();
                _response.Result = attempts;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("quiz/{quizCode}")]
        public async Task<ActionResult<ResponseDto>> GetAttemptsByQuizCode(string quizCode)
        {
            try
            {
                var attempts = await _attemptRepository.GetAttemptsByQuizCodeAsync(quizCode);
                _response.Result = attempts;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<ResponseDto>> GetAttemptsByStudentId(string studentId)
        {
            try
            {
                var attempts = await _attemptRepository.GetAttemptsByStudentIdAsync(studentId);
                _response.Result = attempts;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("{attemptCode}")]
        public async Task<ActionResult<ResponseDto>> GetAttemptByCode(string attemptCode)
        {
            try
            {
                var attempt = await _attemptRepository.GetAttemptByCodeAsync(attemptCode);
                if (attempt == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Attempt with code {attemptCode} not found";
                    return NotFound(_response);
                }
                _response.Result = attempt;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto>> CreateAttempt([FromBody] StudentQuizAttemptCreateDto attemptDto)
        {
            try
            {
                var createdAttempt = await _attemptRepository.CreateAttemptAsync(attemptDto);
                _response.Result = createdAttempt;
                return CreatedAtAction(nameof(GetAttemptByCode), new { attemptCode = createdAttempt.AttemptCode }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpPut]
        public async Task<ActionResult<ResponseDto>> UpdateAttempt([FromBody] StudentQuizAttempt attempt)
        {
            try
            {
                var updatedAttempt = await _attemptRepository.UpdateAttemptAsync(attempt);
                if (updatedAttempt == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Attempt with ID {attempt.Id} not found";
                    return NotFound(_response);
                }
                _response.Result = updatedAttempt;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("{attemptCode}/calculate-score")]
        public async Task<ActionResult<ResponseDto>> CalculateScore(string attemptCode)
        {
            try
            {
                var score = await _attemptRepository.CalculateAndUpdateScoreAsync(attemptCode);
                _response.Result = score;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("professor/{professorId}/recent")]
        public async Task<ActionResult<ResponseDto>> GetRecentAttemptsByProfessorId(string professorId)
        {
            try
            {
                var attempts = await _attemptRepository.GetRecentAttemptsByProfessorIdAsync(professorId);
                _response.Result = attempts;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }
    }
}