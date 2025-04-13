using Microsoft.AspNetCore.Mvc;
using MTS.Services.CurriculumAPI.Models.DTO;
using MTS.Services.CurriculumAPI.Repository.IRepository;

namespace MTS.Services.CurriculumAPI.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentAPIController : ControllerBase
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        protected ResponseDto _response;

        public EnrollmentAPIController(IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository ?? throw new ArgumentNullException(nameof(enrollmentRepository));
            _response = new ResponseDto();
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto>> GetAllEnrollments()
        {
            try
            {
                var enrollments = await _enrollmentRepository.GetAllEnrollmentsAsync();
                _response.Result = enrollments;
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
        public async Task<ActionResult<ResponseDto>> GetEnrollmentsByStudentId(string studentUniversityId)
        {
            try
            {
                var enrollments = await _enrollmentRepository.GetEnrollmentsByStudentIdAsync(studentUniversityId);
                _response.Result = enrollments;
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
        public async Task<ActionResult<ResponseDto>> GetEnrollmentsByCourseCode(string courseCode)
        {
            try
            {
                var enrollments = await _enrollmentRepository.GetEnrollmentsByCourseCodeAsync(courseCode);
                _response.Result = enrollments;
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
        public async Task<ActionResult<ResponseDto>> CreateEnrollment([FromBody] EnrollmentCreateDto enrollmentDto)
        {
            try
            {
                var enrollment = await _enrollmentRepository.CreateEnrollmentAsync(enrollmentDto);
                _response.Result = enrollment;
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
        public async Task<ActionResult<ResponseDto>> UpdateEnrollment([FromBody] EnrollmentUpdateDto enrollmentDto)
        {
            try
            {
                var enrollment = await _enrollmentRepository.UpdateEnrollmentAsync(enrollmentDto);
                if (enrollment == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Enrollment not found";
                    return NotFound(_response);
                }

                _response.Result = enrollment;
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
        public async Task<ActionResult<ResponseDto>> DeleteEnrollment(int id)
        {
            try
            {
                var result = await _enrollmentRepository.DeleteEnrollmentAsync(id);
                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Enrollment not found";
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

        [HttpGet("check/{courseCode}/{studentUniversityId}")]
        public async Task<ActionResult<ResponseDto>> CheckEnrollment(string courseCode, string studentUniversityId)
        {
            try
            {
                var isEnrolled = await _enrollmentRepository.IsStudentEnrolledAsync(courseCode, studentUniversityId);
                _response.Result = isEnrolled;
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