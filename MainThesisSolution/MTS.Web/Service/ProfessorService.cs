using MTS.Web.Models;
using MTS.Web.Models.User.Professor;
using MTS.Web.Service.IService;
using MTS.Web.Utility;

namespace MTS.Web.Service
{
    public class ProfessorService : IProfessorService
    {
        private readonly IBaseService _baseService;

        public ProfessorService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> GetAllProfessorsAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.UserAPIBase + "/api/professors"
            });
        }

        public async Task<ResponseDto?> GetProfessorByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.UserAPIBase}/api/professors/{id}"
            });
        }

        public async Task<ResponseDto?> CreateProfessorAsync(ProfessorCreateDto professorDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = professorDto,
                Url = SD.UserAPIBase + "/api/professors"
            });
        }

        public async Task<ResponseDto?> UpdateProfessorAsync(ProfessorDto professorDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = professorDto,
                Url = $"{SD.UserAPIBase}/api/professors/{professorDto.ID}"
            });
        }

        public async Task<ResponseDto?> DeleteProfessorAsync(int id)
        {
            var temp = $"{SD.UserAPIBase}/api/professors/{id}";

            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = $"{SD.UserAPIBase}/api/professors/{id}"
            });
        }
    }
}