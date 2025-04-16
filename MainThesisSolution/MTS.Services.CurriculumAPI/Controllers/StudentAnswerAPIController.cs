using Microsoft.AspNetCore.Mvc;
using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO;
using MTS.Services.CurriculumAPI.Models.DTO.StudentAnswer;
using MTS.Services.CurriculumAPI.Models.DTO.StudentAnswerDto;
using MTS.Services.CurriculumAPI.Repository.IRepository;

namespace MTS.Services.CurriculumAPI.Controllers
{
    [Route("api/studentanswers")]
    [ApiController]
    public class StudentAnswerAPIController : ControllerBase
    {
        private readonly IStudentAnswerRepository _studentAnswerRepository;
        protected ResponseDto _response;

        public StudentAnswerAPIController(IStudentAnswerRepository studentAnswerRepository)
        {
            _studentAnswerRepository = studentAnswerRepository ?? throw new ArgumentNullException(nameof(studentAnswerRepository));
            _response = new ResponseDto();
        }

        [HttpGet("attempt/{attemptCode}")]
        public async Task<ActionResult<ResponseDto>> GetAnswersByAttemptCode(string attemptCode)
        {
            try
            {
                var answers = await _studentAnswerRepository.GetAnswersByAttemptCodeAsync(attemptCode);
                _response.Result = answers;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("question/{questionCode}")]
        public async Task<ActionResult<ResponseDto>> GetAnswersByQuestionCode(string questionCode)
        {
            try
            {
                var answers = await _studentAnswerRepository.GetAnswersByQuestionCodeAsync(questionCode);
                _response.Result = answers;
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
        public async Task<ActionResult<ResponseDto>> GetAnswersByStudentId(string studentId)
        {
            try
            {
                var answers = await _studentAnswerRepository.GetAnswersByStudentIdAsync(studentId);
                _response.Result = answers;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ResponseDto>> GetAnswerById(int id)
        {
            try
            {
                var answer = await _studentAnswerRepository.GetAnswerByIdAsync(id);
                if (answer == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Student answer with ID {id} not found";
                    return NotFound(_response);
                }
                _response.Result = answer;
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
        public async Task<ActionResult<ResponseDto>> CreateStudentAnswer([FromBody] StudentAnswerCreateDto answerDto)
        {
            try
            {
                var createdAnswer = await _studentAnswerRepository.CreateStudentAnswerAsync(answerDto);
                _response.Result = createdAnswer;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpPut]
        public async Task<ActionResult<ResponseDto>> UpdateStudentAnswer([FromBody] StudentAnswer answer)
        {
            try
            {
                var updatedAnswer = await _studentAnswerRepository.UpdateStudentAnswerAsync(answer);
                if (updatedAnswer == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Student answer with ID {answer.Id} not found";
                    return NotFound(_response);
                }
                _response.Result = updatedAnswer;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpPost("grade")]
        public async Task<ActionResult<ResponseDto>> GradeStudentAnswer([FromBody] StudentAnswerGradeDto gradeDto)
        {
            try
            {
                var studentAnswer = await _studentAnswerRepository.GradeStudentAnswerAsync(gradeDto);
                if (studentAnswer == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Student answer with ID {gradeDto.Id} not found";
                    return NotFound(_response);
                }
                _response.Result = studentAnswer;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("ungraded/professor/{professorId}")]
        public async Task<ActionResult<ResponseDto>> GetUngradedAnswers(string professorId)
        {
            try
            {
                var answers = await _studentAnswerRepository.GetUngradedAnswersAsync(professorId);
                _response.Result = answers;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ResponseDto>> DeleteStudentAnswer(int id)
        {
            try
            {
                var result = await _studentAnswerRepository.DeleteStudentAnswerAsync(id);
                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Student answer with ID {id} not found";
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
    }
}