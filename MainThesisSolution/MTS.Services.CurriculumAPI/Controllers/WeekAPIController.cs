using Microsoft.AspNetCore.Mvc;
using MTS.Services.CurriculumAPI.Models.DTO;
using MTS.Services.CurriculumAPI.Repository.IRepository;

namespace MTS.Services.CurriculumAPI.Controllers
{
    [Route("api/weeks")]
    [ApiController]
    public class WeekController : ControllerBase
    {
        private readonly IWeekRepository _weekRepository;
        protected ResponseDto _response;

        public WeekController(IWeekRepository weekRepository)
        {
            _weekRepository = weekRepository ?? throw new ArgumentNullException(nameof(weekRepository));
            _response = new ResponseDto();
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto>> GetAllWeeks()
        {
            try
            {
                var weeks = await _weekRepository.GetAllWeeksAsync();
                _response.Result = weeks;
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
        public async Task<ActionResult<ResponseDto>> GetWeekById(int id)
        {
            try
            {
                var week = await _weekRepository.GetWeekByIdAsync(id);
                if (week == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Week with ID {id} not found";
                    return NotFound(_response);
                }
                _response.Result = week;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("code/{weekCode}")]
        public async Task<ActionResult<ResponseDto>> GetWeekByCode(string weekCode)
        {
            try
            {
                var week = await _weekRepository.GetWeekByCodeAsync(weekCode);
                if (week == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Week with code {weekCode} not found";
                    return NotFound(_response);
                }
                _response.Result = week;
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
        public async Task<ActionResult<ResponseDto>> GetWeeksByCourseCode(string courseCode)
        {
            try
            {
                var weeks = await _weekRepository.GetWeeksByCourseCodeAsync(courseCode);

                if (weeks == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Course with code {courseCode} not found";
                    return NotFound(_response);
                }

                _response.Result = weeks;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }  

        [HttpGet("{weekCode}/materials")]
        public async Task<ActionResult<ResponseDto>> GetMaterialsByWeekCode(string weekCode)
        {
            try
            {
                var materials = await _weekRepository.GetMaterialsByWeekCodeAsync(weekCode);
                _response.Result = materials;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("{weekCode}/assignments")]
        public async Task<ActionResult<ResponseDto>> GetAssignmentsByWeekCode(string weekCode)
        {
            try
            {
                var assignments = await _weekRepository.GetAssignmentsByWeekCodeAsync(weekCode);
                _response.Result = assignments;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("{weekCode}/quizzes")]
        public async Task<ActionResult<ResponseDto>> GetQuizzesByWeekCode(string weekCode)
        {
            try
            {
                var quizzes = await _weekRepository.GetQuizzesByWeekCodeAsync(weekCode);
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
        public async Task<ActionResult<ResponseDto>> CreateWeek([FromBody] WeekCreateDto weekDto)
        {
            try
            {
                // Only CourseCode is required from client
                if (string.IsNullOrEmpty(weekDto.CourseCode))
                {
                    _response.IsSuccess = false;
                    _response.Message = "Course code is required";
                    return BadRequest(_response);
                }

                var week = await _weekRepository.CreateWeekAsync(weekDto);
                _response.Result = week;
                return CreatedAtAction(nameof(GetWeekByCode), new { weekCode = week.WeekCode }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpPut]
        public async Task<ActionResult<ResponseDto>> UpdateWeek([FromBody] WeekUpdateDto weekDto)
        {
            try
            {
                var week = await _weekRepository.UpdateWeekAsync(weekDto);
                if (week == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Week with code {weekDto.WeekCode} not found";
                    return NotFound(_response);
                }
                _response.Result = week;
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
        public async Task<ActionResult<ResponseDto>> DeleteWeek(int id)
        {
            try
            {
                var result = await _weekRepository.DeleteWeekAsync(id);
                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Week with ID {id} not found";
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