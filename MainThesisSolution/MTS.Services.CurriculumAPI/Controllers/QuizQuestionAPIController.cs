using Microsoft.AspNetCore.Mvc;
using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO;
using MTS.Services.CurriculumAPI.Models.DTO.QuizDto;
using MTS.Services.CurriculumAPI.Repository.IRepository;

namespace MTS.Services.CurriculumAPI.Controllers
{//CURRENT
    [Route("api/questions")]
    [ApiController]
    public class QuizQuestionAPIController : ControllerBase
    {
        private readonly IQuizQuestionRepository _questionRepository;
        protected ResponseDto _response;

        public QuizQuestionAPIController(IQuizQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository ?? throw new ArgumentNullException(nameof(questionRepository));
            _response = new ResponseDto();
        }

        [HttpGet("quiz/{quizCode}")]
        public async Task<ActionResult<ResponseDto>> GetQuestionsByQuizCode(string quizCode)
        {
            try
            {
                var questions = await _questionRepository.GetQuestionsByQuizCodeAsync(quizCode);
                _response.Result = questions;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("code/{questionCode}")]
        public async Task<ActionResult<ResponseDto>> GetQuestionByCode(string questionCode)
        {
            try
            {
                var question = await _questionRepository.GetQuestionByCodeAsync(questionCode);
                if (question == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Question with code {questionCode} not found";
                    return NotFound(_response);
                }
                _response.Result = question;
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
        public async Task<ActionResult<ResponseDto>> CreateQuestion([FromBody] QuizQuestionCreateDto questionDto)
        {
            try
            {
                var createdQuestion = await _questionRepository.CreateQuestionAsync(questionDto);
                _response.Result = createdQuestion;
                return CreatedAtAction(nameof(GetQuestionByCode), new { questionCode = createdQuestion.QuizQuestionCode }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpPut]
        public async Task<ActionResult<ResponseDto>> UpdateQuestion([FromBody] QuizQuestionUpdateDto questionDto)
        {
            try
            {
                var updatedQuestion = await _questionRepository.UpdateQuestionAsync(questionDto);
                if (updatedQuestion == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Question with code {questionDto.QuizQuestionCode} not found";
                    return NotFound(_response);
                }
                _response.Result = updatedQuestion;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpDelete("code/{questionCode}")]
        public async Task<ActionResult<ResponseDto>> DeleteQuestionByCode(string questionCode)
        {
            try
            {
                var result = await _questionRepository.DeleteQuestionByCodeAsync(questionCode);
                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Question with code {questionCode} not found";
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
}