using MTS.Web.Models;
using MTS.Web.Models.Curriculum.Quiz;
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

        public async Task<ResponseDto?> DeleteQuizAsync(string quizCode)
        {
            // Find quiz ID from quizCode first
            var quizResponse = await GetQuizByCodeAsync(quizCode);
            if (quizResponse != null && quizResponse.IsSuccess)
            {
                var quiz = Newtonsoft.Json.JsonConvert.DeserializeObject<QuizDto>(Convert.ToString(quizResponse.Result));

                return await _baseService.SendAsync(new RequestDto()
                {
                    ApiType = SD.ApiType.DELETE,
                    Url = $"{SD.CurriculumAPIBase}/api/quizzes/{quiz.Id}"
                });
            }

            return new ResponseDto { IsSuccess = false, Message = "Quiz not found" };
        }

        public async Task<ResponseDto?> GetQuestionsByQuizCodeAsync(string quizCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/quizzes/{quizCode}/questions"
            });
        }

        public async Task<ResponseDto?> GetQuestionByCodeAsync(string questionCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/quizzes/question/{questionCode}"
            });
        }

        public async Task<ResponseDto?> CreateQuestionAsync(QuizQuestionCreateDto questionDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = questionDto,
                Url = SD.CurriculumAPIBase + "/api/quizzes/question"
            });
        }

        public async Task<ResponseDto?> GetAttemptsByQuizCodeAsync(string quizCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/quizzes/attempts/quiz/{quizCode}"
            });
        }

        public async Task<ResponseDto?> GetAttemptsByStudentIdAsync(string studentUniversityId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/quizzes/attempts/student/{studentUniversityId}"
            });
        }

        public async Task<ResponseDto?> CreateAttemptAsync(StudentQuizAttemptCreateDto attemptDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = attemptDto,
                Url = SD.CurriculumAPIBase + "/api/quizzes/attempt"
            });
        }
    }
}