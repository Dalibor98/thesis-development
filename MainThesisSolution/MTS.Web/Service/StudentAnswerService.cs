using MTS.Web.Models.Curriculum.Quiz.Used;
using MTS.Web.Models;
using MTS.Web.Service.IService;
using MTS.Web.Utility;

namespace MTS.Web.Service
{
    public class StudentAnswerService : IStudentAnswerService
    {
        private readonly IBaseService _baseService;

        public StudentAnswerService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> GetAnswersByAttemptCodeAsync(string attemptCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/studentanswers/attempt/{attemptCode}"
            });
        }

        public async Task<ResponseDto?> GetAnswersByQuestionCodeAsync(string questionCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/studentanswers/question/{questionCode}"
            });
        }

        public async Task<ResponseDto?> GetAnswersByStudentIdAsync(string studentId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/studentanswers/student/{studentId}"
            });
        }

        public async Task<ResponseDto?> GetAnswerByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/studentanswers/{id}"
            });
        }

        public async Task<ResponseDto?> CreateStudentAnswerAsync(StudentAnswerCreateDto answerDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = answerDto,
                Url = SD.CurriculumAPIBase + "/api/studentanswers"
            });
        }

        public async Task<ResponseDto?> UpdateStudentAnswerAsync(StudentAnswerUpdateDto answer)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = answer,
                Url = SD.CurriculumAPIBase + "/api/studentanswers"
            });
        }

        public async Task<ResponseDto?> GradeStudentAnswerAsync(StudentAnswerGradeDto gradeDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = gradeDto,
                Url = SD.CurriculumAPIBase + "/api/studentanswers/grade"
            });
        }

        public async Task<ResponseDto?> GetUngradedAnswersAsync(string professorId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/studentanswers/ungraded/professor/{professorId}"
            });
        }

        public async Task<ResponseDto?> DeleteStudentAnswerAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.CurriculumAPIBase + $"/api/studentanswers/{id}"
            });
        }
    }
}
