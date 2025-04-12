using Microsoft.AspNetCore.Mvc;
using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO;
using MTS.Services.CurriculumAPI.Repository.IRepository;

namespace MTS.Services.CurriculumAPI.Controllers
{
    [Route("api/materials")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialRepository _materialRepository;
        protected ResponseDto _response;

        public MaterialController(IMaterialRepository materialRepository)
        {
            _materialRepository = materialRepository ?? throw new ArgumentNullException(nameof(materialRepository));
            _response = new ResponseDto();
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto>> GetAllMaterials()
        {
            try
            {
                var materials = await _materialRepository.GetAllMaterialsAsync();
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

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ResponseDto>> GetMaterialById(int id)
        {
            try
            {
                var material = await _materialRepository.GetMaterialByIdAsync(id);
                if (material == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Material with ID {id} not found";
                    return NotFound(_response);
                }
                _response.Result = material;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("code/{materialCode}")]
        public async Task<ActionResult<ResponseDto>> GetMaterialByCode(string materialCode)
        {
            try
            {
                var material = await _materialRepository.GetMaterialByCodeAsync(materialCode);
                if (material == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Material with code {materialCode} not found";
                    return NotFound(_response);
                }
                _response.Result = material;
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
        public async Task<ActionResult<ResponseDto>> GetMaterialsByCourseCode(string courseCode)
        {
            try
            {
                var materials = await _materialRepository.GetMaterialsByCourseCodeAsync(courseCode);
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

        [HttpGet("week/{weekCode}")]
        public async Task<ActionResult<ResponseDto>> GetMaterialsByWeekCode(string weekCode)
        {
            try
            {
                var materials = await _materialRepository.GetMaterialsByWeekCodeAsync(weekCode);
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

        [HttpPost]
        public async Task<ActionResult<ResponseDto>> CreateMaterial([FromBody]  MaterialCreateDto materialDto)
        {
            try
            {
                var material = await _materialRepository.CreateMaterialAsync(materialDto);
                _response.Result = material;
                return CreatedAtAction(nameof(GetMaterialByCode), new { materialCode = material.MaterialCode }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpPut]
        public async Task<ActionResult<ResponseDto>> UpdateMaterial([FromBody] MaterialUpdateDto materialDto)
        {
            try
            {
                var material = await _materialRepository.UpdateMaterialAsync(materialDto);
                if (material == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Material with code {materialDto.MaterialCode} not found";
                    return NotFound(_response);
                }
                _response.Result = material;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpDelete("code/{materialCode}")]
        public async Task<ActionResult<ResponseDto>> DeleteMaterialByCode(string materialCode)
        {
            try
            {
                var result = await _materialRepository.DeleteMaterialByCodeAsync(materialCode);
                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Material with code {materialCode} not found";
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