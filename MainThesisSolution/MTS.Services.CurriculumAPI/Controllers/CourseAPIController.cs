using Microsoft.AspNetCore.Mvc;
using MTS.Services.CurriculumAPI.Models.DTO;
using MTS.Services.CurriculumAPI.Repository.IRepository;
namespace MTS.Services.CurriculumAPI.Controllers
{
    [Route("api/courses")]
    [ApiController]
    public class CourseAPIController : ControllerBase
    {
        private readonly ICourseRepository _courseRepository;
        protected ResponseDto _response;
        public CourseAPIController(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
            _response = new ResponseDto();
        }
        [HttpGet]
        public async Task<ActionResult<ResponseDto>> GetCourses()
        {
            try
            {
                var courses = await _courseRepository.GetAllCoursesAsync();
                _response.Result = courses;
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
        public async Task<ActionResult<ResponseDto>> GetCourseById(int id)
        {
            try
            {
                var course = await _courseRepository.GetCourseByIdAsync(id);
                if (course == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Course with ID {id} not found";
                    return NotFound(_response);
                }
                _response.Result = course;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }
        [HttpGet("code/{courseCode}")]
        public async Task<ActionResult<ResponseDto>> GetCourseByCode(string courseCode)
        {
            try
            {
                var course = await _courseRepository.GetCourseByCodeAsync(courseCode);
                if (course == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Course with code {courseCode} not found";
                    return NotFound(_response);
                }
                _response.Result = course;
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
        public async Task<ActionResult<ResponseDto>> CreateCourse([FromBody] TemporaryCourseDTO courseCreateDto)
        {
            try
            {
                var createdCourse = await _courseRepository.CreateCourseAsync(courseCreateDto);
                _response.Result = createdCourse;
                return CreatedAtAction(nameof(GetCourseByCode), new { courseCode = createdCourse.CourseCode }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }
        
        [HttpPut]
        public async Task<ActionResult<ResponseDto>> UpdateCourse([FromBody] CourseUpdateDto course)
        {
            try
            {
                var updatedCourse = await _courseRepository.UpdateCourseAsync(course);
                if (updatedCourse == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Course with code {course.CourseCode} not found";
                    return NotFound(_response);
                }
                _response.Result = updatedCourse;
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
        public async Task<ActionResult<ResponseDto>> DeleteCourse(int id)
        {
            try
            {
                var result = await _courseRepository.DeleteCourseAsync(id);
                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Course with ID {id} not found";
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

        [HttpGet("{courseCode}/weeks")]
        public async Task<ActionResult<ResponseDto>> GetWeeksByCourseCode(string courseCode)
        {
            try
            {
                var weeks = await _courseRepository.GetWeeksByCourseCodeAsync(courseCode);
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

        [HttpGet("{courseCode}/materials")]
        public async Task<ActionResult<ResponseDto>> GetMaterialsByCourseCode(string courseCode)
        {
            try
            {
                var materials = await _courseRepository.GetMaterialsByCourseCodeAsync(courseCode);
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

        [HttpGet("{courseCode}/assignments")]
        public async Task<ActionResult<ResponseDto>> GetAssignmentsByCourseCode(string courseCode)
        {
            try
            {
                var assignments = await _courseRepository.GetAssignmentsByCourseCodeAsync(courseCode);
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

        [HttpGet("{courseCode}/quizzes")]
        public async Task<ActionResult<ResponseDto>> GetQuizzesByCourseCode(string courseCode)
        {
            try
            {
                var quizzes = await _courseRepository.GetQuizzesByCourseCodeAsync(courseCode);
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

        [HttpGet("{courseCode}/registrations")]
        public async Task<ActionResult<ResponseDto>> GetRegistrationsByCourseCode(string courseCode)
        {
            try
            {
                var registrations = await _courseRepository.GetRegistrationsByCourseCodeAsync(courseCode);
                _response.Result = registrations;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("professor/{professorUniversityId}")]
        public async Task<ActionResult<ResponseDto>> GetCoursesByProfessorId(string professorUniversityId)
        {
            try
            {
                var courses = await _courseRepository.GetCoursesByProfessorIdAsync(professorUniversityId);
                _response.Result = courses;
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