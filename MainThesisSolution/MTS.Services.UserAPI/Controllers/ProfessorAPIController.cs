using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MTS.Services.UserAPI.Models.DTO;
using MTS.Services.UserAPI.Repository.IRepository;

namespace MTS.Services.UserAPI.Controllers
{
    
    [Route("api/professors")]
    [ApiController]
    public class ProfessorAPIController : ControllerBase
    {
        private readonly IProfessorRepository _professorRepository;
        protected ResponseDto _response;

        public ProfessorAPIController(IProfessorRepository professorRepository)
        {
            _professorRepository = professorRepository;
            _response = new ResponseDto();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var professors = await _professorRepository.GetAllProfessorsAsync();
                _response.Result = professors;
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
                var professor = await _professorRepository.GetProfessorByIdAsync(id);

                if (professor == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Professor not found";
                    return NotFound(_response);
                }

                _response.Result = professor;
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
        public async Task<IActionResult> Post([FromBody] ProfessorCreateDto professorDto)
        {
            try
            {
                var professor = await _professorRepository.CreateProfessorAsync(professorDto);

                if (professor == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Invalid university ID or university ID already in use";
                    return BadRequest(_response);
                }

                _response.Result = professor;
                return CreatedAtAction(nameof(Get), new { id = professor.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ProfessorCreateDto professorDto)
        {
            try
            {
                var success = await _professorRepository.UpdateProfessorAsync(id, professorDto);

                if (!success)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Professor not found";
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
                var success = await _professorRepository.DeleteProfessorAsync(id);

                if (!success)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Professor not found";
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

        [HttpGet("verify/{universityId}")]
        public async Task<IActionResult> VerifyProfessor(string universityId)
        {
            try
            {
                var professor = await _professorRepository.GetProfessorByUniversityIdAsync(universityId);
                _response.Result = professor != null;
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
}
