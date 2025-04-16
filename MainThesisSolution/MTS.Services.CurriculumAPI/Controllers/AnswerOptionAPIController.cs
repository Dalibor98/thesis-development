using Microsoft.AspNetCore.Mvc;
using MTS.Services.CurriculumAPI.Models.DTO;
using MTS.Services.CurriculumAPI.Models.DTO.AnswerOption;
using MTS.Services.CurriculumAPI.Repository.IRepository;

namespace MTS.Services.CurriculumAPI.Controllers
{
    [Route("api/answeroptions")]
    [ApiController]
    public class AnswerOptionAPIController : ControllerBase
    {//CURRENT
        private readonly IAnswerOptionRepository _answerOptionRepository;
        protected ResponseDto _response;

        public AnswerOptionAPIController(IAnswerOptionRepository answerOptionRepository)
        {
            _answerOptionRepository = answerOptionRepository ?? throw new ArgumentNullException(nameof(answerOptionRepository));
            _response = new ResponseDto();
        }

        [HttpGet("question/{questionCode}")]
        public async Task<ActionResult<ResponseDto>> GetOptionsByQuestionCode(string questionCode)
        {
            try
            {
                var options = await _answerOptionRepository.GetOptionsByQuestionCodeAsync(questionCode);
                _response.Result = options;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("{optionCode}")]
        public async Task<ActionResult<ResponseDto>> GetOptionByCode(string optionCode)
        {
            try
            {
                var option = await _answerOptionRepository.GetOptionByCodeAsync(optionCode);
                if (option == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Option with code {optionCode} not found";
                    return NotFound(_response);
                }
                _response.Result = option;
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
        public async Task<ActionResult<ResponseDto>> CreateOption([FromBody] AnswerOptionCreateDto optionDto)
        {
            try
            {
                var createdOption = await _answerOptionRepository.CreateOptionAsync(optionDto);
                _response.Result = createdOption;
                return CreatedAtAction(nameof(GetOptionByCode), new { optionCode = createdOption.OptionCode }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpPut]
        public async Task<ActionResult<ResponseDto>> UpdateOption([FromBody] AnswerOptionUpdateDto optionDto)
        {
            try
            {
                var updatedOption = await _answerOptionRepository.UpdateOptionAsync(optionDto);
                if (updatedOption == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Option with ID {optionDto.Id} not found";
                    return NotFound(_response);
                }
                _response.Result = updatedOption;
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
        public async Task<ActionResult<ResponseDto>> DeleteOption(int id)
        {
            try
            {
                var result = await _answerOptionRepository.DeleteOptionAsync(id);
                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Option with ID {id} not found";
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

        [HttpDelete("code/{optionCode}")]
        public async Task<ActionResult<ResponseDto>> DeleteOptionByCode(string optionCode)
        {
            try
            {
                var result = await _answerOptionRepository.DeleteOptionByCodeAsync(optionCode);
                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Option with code {optionCode} not found";
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

        [HttpGet("question/{questionCode}/correct")]
        public async Task<ActionResult<ResponseDto>> GetCorrectOptionsForQuestion(string questionCode)
        {
            try
            {
                var options = await _answerOptionRepository.GetCorrectOptionsForQuestionAsync(questionCode);
                _response.Result = options;
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