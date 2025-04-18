using MTS.Web.Models;
using MTS.Web.Models.Curriculum.Quiz;
using MTS.Web.Service.IService;
using MTS.Web.Utility;

namespace MTS.Web.Service
{
    public class AnswerOptionService : IAnswerOptionService
    {
        private readonly IBaseService _baseService;

        public AnswerOptionService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> GetOptionsByQuestionCodeAsync(string questionCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/answeroptions/question/{questionCode}"
            });
        }

        public async Task<ResponseDto?> GetOptionByCodeAsync(string optionCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/answeroptions/{optionCode}"
            });
        }

        public async Task<ResponseDto?> CreateOptionAsync(AnswerOptionCreateDto optionDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = optionDto,
                Url = SD.CurriculumAPIBase + "/api/answeroptions"
            });
        }

        public async Task<ResponseDto?> UpdateOptionAsync(AnswerOptionUpdateDto optionDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = optionDto,
                Url = SD.CurriculumAPIBase + "/api/answeroptions"
            });
        }

        public async Task<ResponseDto?> DeleteOptionAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.CurriculumAPIBase + $"/api/answeroptions/{id}"
            });
        }

        public async Task<ResponseDto?> DeleteOptionByCodeAsync(string optionCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.CurriculumAPIBase + $"/api/answeroptions/code/{optionCode}"
            });
        }

        public async Task<ResponseDto?> GetCorrectOptionsForQuestionAsync(string questionCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/answeroptions/question/{questionCode}/correct"
            });
        }
    }
}