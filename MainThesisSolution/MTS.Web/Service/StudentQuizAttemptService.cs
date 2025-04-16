using MTS.Web.Models;
using MTS.Web.Models.Curriculum.Quiz.Used;
using MTS.Web.Service.IService;
using MTS.Web.Utility;

namespace MTS.Web.Service
{
    public class StudentQuizAttemptService : IStudentQuizAttemptService
    {
        private readonly IBaseService _baseService;

        public StudentQuizAttemptService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> GetAttemptsByQuizCodeAsync(string quizCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/quizattempts/quiz/{quizCode}"
            });
        }

        public async Task<ResponseDto?> GetAttemptsByStudentIdAsync(string studentUniversityId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/quizattempts/student/{studentUniversityId}"
            });
        }

        public async Task<ResponseDto?> GetAttemptByCodeAsync(string attemptCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/quizattempts/{attemptCode}"
            });
        }

        public async Task<ResponseDto?> CreateAttemptAsync(StudentQuizAttemptCreateDto attempt)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = attempt,
                Url = SD.CurriculumAPIBase + "/api/quizattempts"
            });
        }

        public async Task<ResponseDto?> UpdateAttemptAsync(StudentQuizAttemptUpdateDto attempt)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = attempt,
                Url = SD.CurriculumAPIBase + "/api/quizattempts"
            });
        }

        public async Task<ResponseDto?> GetRecentAttemptsByProfessorIdAsync(string professorId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/quizattempts/professor/{professorId}/recent"
            });
        }

        public async Task<ResponseDto?> CalculateAndUpdateScoreAsync(string attemptCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/quizattempts/{attemptCode}/calculate-score"
            });
        }
    }
}
