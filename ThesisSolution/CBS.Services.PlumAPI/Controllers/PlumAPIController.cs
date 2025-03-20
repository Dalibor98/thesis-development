using CBS.Services.PlumAPI.Models;
using CBS.Services.PlumAPI.Models.DTO;
using CBS.Services.PlumAPI.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CBS.Services.PlumAPI.Models.Dto;
using CBS.Services.PlumAPI.Repository.IRepository;

namespace CBS.Services.PlumAPI.Controllers
{
    [Route("api/plum")]
    [ApiController]
    public class PlumAPIController : ControllerBase
    {
        private readonly IPlumRepository _repository;
        private readonly IMapper _mapper;
        private ResponseDto _response;

        public PlumAPIController(IPlumRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
            _response = new ResponseDto();
        }

        [HttpGet]
        [Authorize]
        public async Task<ResponseDto> Get()
        {
            try
            {
                var plums = await _repository.GetAllPlumsAsync();
                _response.Result = _mapper.Map<IEnumerable<PlumDto>>(plums);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ResponseDto> Get(int id)
        {
            try
            {
                var plum = await _repository.GetPlumByIdAsync(id);
                _response.Result = _mapper.Map<PlumDto>(plum);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        [Authorize(Roles = "LEADER")]
        public async Task<ResponseDto> Post([FromBody] PlumDto plumDto)
        {
            try
            {
                var plum = _mapper.Map<Plum>(plumDto);
                var createdPlum = await _repository.CreatePlumAsync(plum);
                _response.Result = _mapper.Map<PlumDto>(createdPlum);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPut]
        [Authorize(Roles = "LEADER")]
        public async Task<ResponseDto> Put([FromBody] PlumDto plumDto)
        {
            try
            {
                var plum = _mapper.Map<Plum>(plumDto);
                var updatedPlum = await _repository.UpdatePlumAsync(plum);
                _response.Result = _mapper.Map<PlumDto>(updatedPlum);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "LEADER")]
        public async Task<ResponseDto> Delete(int id)
        {
            try
            {
                var plumToDelete = _repository.GetPlumByIdAsync(id);

                if (plumToDelete == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Plum not found.";
                    return _response;
                }
                
                
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
