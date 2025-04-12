using MTS.Web.Models;
using MTS.Web.Models.Curriculum.Week;
using MTS.Web.Service.IService;
using MTS.Web.Utility;

namespace MTS.Web.Service
{
    public class WeekService : IWeekService
    {
        private readonly IBaseService _baseService;

        public WeekService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> GetWeekByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.CurriculumAPIBase}/api/weeks/{id}"
            });
        }

        public async Task<ResponseDto?> GetWeekByCodeAsync(string weekCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.CurriculumAPIBase}/api/weeks/code/{weekCode}"
            });
        }

        public async Task<ResponseDto?> GetWeeksByCourseCodeAsync(string courseCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.CurriculumAPIBase}/api/weeks/course/{courseCode}"
            });
        }

        public async Task<ResponseDto?> CreateWeekAsync(WeekCreateDto weekDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = weekDto,
                Url = $"{SD.CurriculumAPIBase}/api/weeks"
            });
        }

        public async Task<ResponseDto?> UpdateWeekAsync(WeekUpdateDto weekUpdateDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = weekUpdateDto,
                Url = $"{SD.CurriculumAPIBase}/api/weeks"
            });
        }

        public async Task<ResponseDto?> DeleteWeekAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = $"{SD.CurriculumAPIBase}/api/weeks/{id}"
            });
        }

        public async Task<ResponseDto?> DeleteWeekByCodeAsync(string weekCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = $"{SD.CurriculumAPIBase}/api/weeks/code/{weekCode}"
            });
        }

        public async Task<ResponseDto?> GetMaterialsByWeekCodeAsync(string weekCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.CurriculumAPIBase}/api/weeks/{weekCode}/materials"
            });
        }

        public async Task<ResponseDto?> GetAssignmentsByWeekCodeAsync(string weekCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.CurriculumAPIBase}/api/weeks/{weekCode}/assignments"
            });
        }

        public async Task<ResponseDto?> GetQuizzesByWeekCodeAsync(string weekCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.CurriculumAPIBase}/api/weeks/{weekCode}/quizzes"
            });
        }
    }
}