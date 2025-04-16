using MTS.Web.Models;
using MTS.Web.Models.Curriculum.Quiz.Used;
using MTS.Web.Service.IService;
using MTS.Web.Utility;

namespace MTS.Web.Service
{
    public class QuizService : IQuizService
    {
        private readonly IBaseService _baseService;

        public QuizService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> GetAllQuizzesAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + "/api/quizzes"
            });
        }

        public async Task<ResponseDto?> GetQuizByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/quizzes/{id}"
            });
        }

        public async Task<ResponseDto?> GetQuizByCodeAsync(string quizCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/quizzes/code/{quizCode}"
            });
        }

        public async Task<ResponseDto?> GetQuizzesByWeekCodeAsync(string weekCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/quizzes/week/{weekCode}"
            });
        }

        public async Task<ResponseDto?> GetQuizzesByCourseCodeAsync(string courseCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/quizzes/course/{courseCode}"
            });
        }

        public async Task<ResponseDto?> CreateQuizAsync(QuizCreateDto quizDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = quizDto,
                Url = SD.CurriculumAPIBase + "/api/quizzes"
            });
        }

        public async Task<ResponseDto?> UpdateQuizAsync(QuizUpdateDto quizDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = quizDto,
                Url = SD.CurriculumAPIBase + "/api/quizzes"
            });
        }

        public async Task<ResponseDto?> DeleteQuizAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.CurriculumAPIBase + $"/api/quizzes/{id}"
            });
        }

        public async Task<ResponseDto?> GetUpcomingQuizzesByStudentIdAsync(string studentId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/quizzes/student/{studentId}/upcoming"
            });
        }
    }
}