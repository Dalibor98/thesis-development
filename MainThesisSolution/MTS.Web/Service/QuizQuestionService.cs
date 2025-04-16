using MTS.Web.Models;
using MTS.Web.Models.Curriculum.Quiz.Used;
using MTS.Web.Service.IService;
using MTS.Web.Utility;

public class QuizQuestionService : IQuizQuestionService
{
    private readonly IBaseService _baseService;

    public QuizQuestionService(IBaseService baseService)
    {
        _baseService = baseService;
    }

    public async Task<ResponseDto?> GetQuestionsByQuizCodeAsync(string quizCode)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.GET,
            Url = $"{SD.CurriculumAPIBase}/api/questions/quiz/{quizCode}"
        });
    }

    public async Task<ResponseDto?> GetQuestionByCodeAsync(string questionCode)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.GET,
            Url = $"{SD.CurriculumAPIBase}/api/questions/code/{questionCode}"
        });
    }

    public async Task<ResponseDto?> CreateQuestionAsync(QuizQuestionCreateDto questionDto)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.POST,
            Data = questionDto,
            Url = $"{SD.CurriculumAPIBase}/api/questions"
        });
    }

    public async Task<ResponseDto?> UpdateQuestionAsync(QuizQuestionUpdateDto questionDto)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.PUT,
            Data = questionDto,
            Url = $"{SD.CurriculumAPIBase}/api/questions"
        });
    }

    public async Task<ResponseDto?> DeleteQuestionAsync(string questionCode)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.DELETE,
            Url = $"{SD.CurriculumAPIBase}/api/questions/code/{questionCode}"
        });
    }
}