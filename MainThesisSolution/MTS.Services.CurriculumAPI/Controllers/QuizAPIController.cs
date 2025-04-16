using Microsoft.AspNetCore.Mvc;
using MTS.Services.CurriculumAPI.Models.DTO;
using MTS.Services.CurriculumAPI.Models.DTO.QuizDto;
using MTS.Services.CurriculumAPI.Repository.IRepository;

[Route("api/quizzes")]
[ApiController]
public class QuizAPIController : ControllerBase
{
    //CURRENT
    private readonly IQuizRepository _quizRepository;
    protected ResponseDto _response;

    public QuizAPIController(IQuizRepository quizRepository)
    {
        _quizRepository = quizRepository ?? throw new ArgumentNullException(nameof(quizRepository));
        _response = new ResponseDto();
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDto>> GetQuizzes()
    {
        try
        {
            var quizzes = await _quizRepository.GetAllQuizzesAsync();
            _response.Result = quizzes;
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
    public async Task<ActionResult<ResponseDto>> GetQuizById(int id)
    {
        try
        {
            var quiz = await _quizRepository.GetQuizByIdAsync(id);
            if (quiz == null)
            {
                _response.IsSuccess = false;
                _response.Message = $"Quiz with ID {id} not found";
                return NotFound(_response);
            }
            _response.Result = quiz;
            return Ok(_response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
            return StatusCode(500, _response);
        }
    }

    [HttpGet("code/{quizCode}")]
    public async Task<ActionResult<ResponseDto>> GetQuizByCode(string quizCode)
    {
        try
        {
            var quiz = await _quizRepository.GetQuizByCodeAsync(quizCode);
            if (quiz == null)
            {
                _response.IsSuccess = false;
                _response.Message = $"Quiz with code {quizCode} not found";
                return NotFound(_response);
            }
            _response.Result = quiz;
            return Ok(_response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
            return StatusCode(500, _response);
        }
    }

    [HttpGet("course/{courseCode}")]
    public async Task<ActionResult<ResponseDto>> GetQuizzesByCourseCode(string courseCode)
    {
        try
        {
            var quizzes = await _quizRepository.GetQuizzesByCourseCodeAsync(courseCode);
            _response.Result = quizzes;
            return Ok(_response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
            return StatusCode(500, _response);
        }
    }

    [HttpGet("week/{weekCode}")]
    public async Task<ActionResult<ResponseDto>> GetQuizzesByWeekCode(string weekCode)
    {
        try
        {
            var quizzes = await _quizRepository.GetQuizzesByWeekCodeAsync(weekCode);
            _response.Result = quizzes;
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
    public async Task<ActionResult<ResponseDto>> CreateQuiz([FromBody] QuizCreateDto quizDto)
    {
        try
        {
            var createdQuiz = await _quizRepository.CreateQuizAsync(quizDto);
            _response.Result = createdQuiz;
            return CreatedAtAction(nameof(GetQuizByCode), new { quizCode = createdQuiz.QuizCode }, _response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
            return StatusCode(500, _response);
        }
    }

    [HttpPut]
    public async Task<ActionResult<ResponseDto>> UpdateQuiz([FromBody] QuizUpdateDto quizDto)
    {
        try
        {
            var updatedQuiz = await _quizRepository.UpdateQuizAsync(quizDto);
            if (updatedQuiz == null)
            {
                _response.IsSuccess = false;
                _response.Message = $"Quiz with code {quizDto.QuizCode} not found";
                return NotFound(_response);
            }
            _response.Result = updatedQuiz;
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
    public async Task<ActionResult<ResponseDto>> DeleteQuiz(int id)
    {
        try
        {
            var result = await _quizRepository.DeleteQuizAsync(id);
            if (!result)
            {
                _response.IsSuccess = false;
                _response.Message = $"Quiz with ID {id} not found";
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

    [HttpGet("student/{studentId}/upcoming")]
    public async Task<ActionResult<ResponseDto>> GetUpcomingQuizzesByStudentId(string studentId)
    {
        try
        {
            var quizzes = await _quizRepository.GetUpcomingQuizzesByStudentIdAsync(studentId);
            _response.Result = quizzes;
            _response.IsSuccess = true;
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