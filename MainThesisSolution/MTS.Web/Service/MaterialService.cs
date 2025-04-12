using MTS.Web.Models;
using MTS.Web.Models.Curriculum.Material;
using MTS.Web.Service.IService;
using MTS.Web.Utility;

namespace MTS.Web.Service
{
    public class MaterialService : IMaterialService
    {
        private readonly IBaseService _baseService;

        public MaterialService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> GetMaterialByCodeAsync(string materialCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/materials/code/{materialCode}"
            });
        }

        public async Task<ResponseDto?> GetMaterialsByWeekCodeAsync(string weekCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/weeks/{weekCode}/materials"
            });
        }

        public async Task<ResponseDto?> GetMaterialsByCourseCodeAsync(string courseCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/courses/{courseCode}/materials"
            });
        }

        public async Task<ResponseDto?> CreateMaterialAsync(MaterialCreateDto materialDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = materialDto,
                Url = SD.CurriculumAPIBase + "/api/materials"
            });
        }

        public async Task<ResponseDto?> UpdateMaterialAsync(MaterialCreateDto materialDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = materialDto,
                Url = SD.CurriculumAPIBase + "/api/materials"
            });
        }

        public async Task<ResponseDto?> DeleteMaterialAsync(string materialCode)
        {
            var temp = SD.CurriculumAPIBase + $"/api/materials/code/{materialCode}";
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.CurriculumAPIBase + $"/api/materials/code/{materialCode}"
            });
        }
    }
}