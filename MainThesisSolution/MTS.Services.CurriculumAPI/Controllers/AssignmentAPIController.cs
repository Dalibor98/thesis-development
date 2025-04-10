using Microsoft.AspNetCore.Mvc;
using MTS.Services.CurriculumAPI.Models.DTO;
using MTS.Services.CurriculumAPI.Repository.IRepository;

namespace MTS.Services.CurriculumAPI.Controllers
{
    [Route("api/assignments")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly IAssignmentRepository _assignmentRepository;
        protected ResponseDto _response;

        public AssignmentController(IAssignmentRepository assignmentRepository)
        {
            _assignmentRepository = assignmentRepository ?? throw new ArgumentNullException(nameof(assignmentRepository));
            _response = new ResponseDto();
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto>> GetAllAssignments()
        {
            try
            {
                var assignments = await _assignmentRepository.GetAllAssignmentsAsync();
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

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ResponseDto>> GetAssignmentById(int id)
        {
            try
            {
                var assignment = await _assignmentRepository.GetAssignmentByIdAsync(id);
                if (assignment == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Assignment with ID {id} not found";
                    return NotFound(_response);
                }
                _response.Result = assignment;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("code/{assignmentCode}")]
        public async Task<ActionResult<ResponseDto>> GetAssignmentByCode(string assignmentCode)
        {
            try
            {
                var assignment = await _assignmentRepository.GetAssignmentByCodeAsync(assignmentCode);
                if (assignment == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Assignment with code {assignmentCode} not found";
                    return NotFound(_response);
                }
                _response.Result = assignment;
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
        public async Task<ActionResult<ResponseDto>> GetAssignmentsByCourseCode(string courseCode)
        {
            try
            {
                var assignments = await _assignmentRepository.GetAssignmentsByCourseCodeAsync(courseCode);
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

        [HttpGet("week/{weekCode}")]
        public async Task<ActionResult<ResponseDto>> GetAssignmentsByWeekCode(string weekCode)
        {
            try
            {
                var assignments = await _assignmentRepository.GetAssignmentsByWeekCodeAsync(weekCode);
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

        [HttpGet("{assignmentCode}/submissions")]
        public async Task<ActionResult<ResponseDto>> GetSubmissionsByAssignmentCode(string assignmentCode)
        {
            try
            {
                var submissions = await _assignmentRepository.GetSubmissionsByAssignmentCodeAsync(assignmentCode);
                _response.Result = submissions;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("{assignmentCode}/student/{studentUniversityId}")]
        public async Task<ActionResult<ResponseDto>> GetStudentSubmission(string assignmentCode, string studentUniversityId)
        {
            try
            {
                var submission = await _assignmentRepository.GetStudentSubmissionAsync(assignmentCode, studentUniversityId);
                if (submission == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Submission for assignment {assignmentCode} by student {studentUniversityId} not found";
                    return NotFound(_response);
                }
                _response.Result = submission;
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
        public async Task<ActionResult<ResponseDto>> CreateAssignment([FromBody] AssignmentCreateDto assignmentDto)
        {
            try
            {
                var assignment = await _assignmentRepository.CreateAssignmentAsync(assignmentDto);
                _response.Result = assignment;
                return CreatedAtAction(nameof(GetAssignmentByCode), new { assignmentCode = assignment.AssignmentCode }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpPut]
        public async Task<ActionResult<ResponseDto>> UpdateAssignment([FromBody] AssignmentUpdateDto assignmentDto)
        {
            try
            {
                var assignment = await _assignmentRepository.UpdateAssignmentAsync(assignmentDto);
                if (assignment == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Assignment with code {assignmentDto.AssignmentCode} not found";
                    return NotFound(_response);
                }
                _response.Result = assignment;
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
        public async Task<ActionResult<ResponseDto>> DeleteAssignment(int id)
        {
            try
            {
                var result = await _assignmentRepository.DeleteAssignmentAsync(id);
                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Assignment with ID {id} not found";
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

        [HttpDelete("code/{assignmentCode}")]
        public async Task<ActionResult<ResponseDto>> DeleteAssignmentByCode(string assignmentCode)
        {
            try
            {
                var result = await _assignmentRepository.DeleteAssignmentByCodeAsync(assignmentCode);
                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Assignment with code {assignmentCode} not found";
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