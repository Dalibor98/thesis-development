using MTS.Web.Models;
using MTS.Web.Service.IService;
using MTS.Web.Utility;

namespace MTS.Web.Service
{
    public class UniversityIdService : IUniversityIdService
    {
        private readonly IBaseService _baseService;

        public UniversityIdService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> GenerateUniversityIdsAsync(string type, int count)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = new { Type = type, Count = count },
                Url = SD.UserAPIBase + "/api/universityids/generate"
            });
        }

        public async Task<ResponseDto?> VerifyUniversityIdAsync(string code, string type)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = new { Code = code, Type = type },
                Url = SD.UserAPIBase + "/api/universityids/verify"
            });
        }

        public async Task<ResponseDto?> GetUnassignedIdsAsync(string type)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.UserAPIBase}/api/universityids/unassigned/{type}"
            });
        }
    }
}