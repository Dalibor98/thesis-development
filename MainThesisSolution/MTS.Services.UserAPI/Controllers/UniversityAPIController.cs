using Microsoft.AspNetCore.Mvc;
using MTS.Services.UserAPI.Models.DTO;
using MTS.Services.UserAPI.Repository.IRepository;

namespace MTS.Services.UserAPI.Controllers
{
    [Route("api/universityids")]
    [ApiController]
    public class UniversityIdAPIController : ControllerBase
    {
        private readonly IUniversityIdRepository _universityIdRepository;
        protected ResponseDto _response;

        public UniversityIdAPIController(IUniversityIdRepository universityIdRepository)
        {
            _universityIdRepository = universityIdRepository;
            _response = new ResponseDto();
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateIds([FromBody] UniversityIdGenerateDto generateDto)
        {
            try
            {
                var generatedIds = await _universityIdRepository.GenerateUniversityIdsAsync(generateDto.Type, generateDto.Count);
                _response.Result = generatedIds;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyId([FromBody] UniversityIdVerifyDto verifyDto)
        {
            try
            {
                var isValid = await _universityIdRepository.VerifyUniversityIdAsync(verifyDto.Code, verifyDto.Type);
                _response.Result = isValid;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }


        [HttpGet("unassigned/{type}")]
        public async Task<IActionResult> GetUnassignedIds(string type)
        {
            try
            {
                var unassignedIds = await _universityIdRepository.GetAllUnassignedIdsAsync(type);
                _response.Result = unassignedIds;
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

