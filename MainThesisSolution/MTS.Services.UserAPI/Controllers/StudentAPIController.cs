using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MTS.Services.UserAPI.Models.DTO;
using MTS.Services.UserAPI.Repository.IRepository;

namespace MTS.Services.UserAPI.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentAPIController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;
        protected ResponseDto _response;

        public StudentAPIController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
            _response = new ResponseDto();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var students = await _studentRepository.GetAllStudentsAsync();
                _response.Result = students;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var student = await _studentRepository.GetStudentByIdAsync(id);

                if (student == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Student not found";
                    return NotFound(_response);
                }

                _response.Result = student;
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
        public async Task<IActionResult> Post([FromBody] StudentCreateDto studentDto)
        {
            try
            {
                var student = await _studentRepository.CreateStudentAsync(studentDto);

                if (student == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Invalid university ID or university ID already in use";
                    return BadRequest(_response);
                }

                _response.Result = student;
                return CreatedAtAction(nameof(Get), new { id = student.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] StudentCreateDto studentDto)
        {
            try
            {
                var success = await _studentRepository.UpdateStudentAsync(id, studentDto);

                if (!success)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Student not found";
                    return NotFound(_response);
                }

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _studentRepository.DeleteStudentAsync(id);

                if (!success)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Student not found";
                    return NotFound(_response);
                }

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
