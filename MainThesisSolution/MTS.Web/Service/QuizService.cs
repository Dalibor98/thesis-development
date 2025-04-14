using MTS.Web.Models;
using MTS.Web.Models.Curriculum.Quiz;
using MTS.Web.Service.IService;
using MTS.Web.Utility;
using Newtonsoft.Json;

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
            var temp = SD.CurriculumAPIBase + "/api/quizzes/attempt";
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = attemptDto,
                Url = SD.CurriculumAPIBase + "/api/quizzes/attempt"
            });
        }
        public async Task<ResponseDto?> GetAttemptByCodeAsync(string attemptCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/quizzes/attempt/{attemptCode}"
            });
        }

        public async Task<ResponseDto?> GetAnswersByAttemptCodeAsync(string attemptCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/quizzes/attempt/{attemptCode}/answers"
            });
        }

        public async Task<ResponseDto?> SaveStudentAnswerAsync(StudentQuizAnswerCreateDto answerDto)
        {
            var temp = SD.CurriculumAPIBase + "/api/quizzes/studentanswer";
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = answerDto,
                Url = SD.CurriculumAPIBase + "/api/quizzes/studentanswer"
            });
        }

        public async Task<ResponseDto?> UpdateAttemptAsync(StudentQuizAttemptDto attemptDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = attemptDto,
                Url = SD.CurriculumAPIBase + "/api/quizzes/attempt"
            });
        }

        public async Task<ResponseDto?> CalculateScoreAsync(string attemptCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/quizzes/attempt/{attemptCode}/score"
            });
        }
        public async Task<ResponseDto?> GetAnswersByCodeAsync(string answerCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/quizzes/answer/{answerCode}"
            });
        }

        public async Task<ResponseDto?> UpdateQuestionAsync(QuizQuestionUpdateDto questionDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = questionDto,
                Url = SD.CurriculumAPIBase + "/api/quizzes/question"
            });
        }

        public async Task<ResponseDto?> DeleteQuestionAsync(string questionCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.CurriculumAPIBase + $"/api/quizzes/question/code/{questionCode}"
            });
        }

        public async Task<ResponseDto?> GetAnswersForQuestionAsync(string questionCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/quizzes/question/{questionCode}/answers"
            });
        }

        public async Task<ResponseDto?> CreateAnswerAsync(AnswerCreateDto answerDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = answerDto,
                Url = SD.CurriculumAPIBase + "/api/quizzes/answer"
            });
        }

        public async Task<ResponseDto?> UpdateAnswerAsync(AnswerUpdateDto answerDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = answerDto,
                Url = SD.CurriculumAPIBase + "/api/quizzes/answer"
            });
        }

        public async Task<ResponseDto?> DeleteAnswerAsync(int answerId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.CurriculumAPIBase + $"/api/quizzes/answer/{answerId}"
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

        public async Task<ResponseDto?> GetRecentQuizAttemptsByProfessorIdAsync(string professorId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/quizzes/professor/{professorId}/attempts"
            });
        }
        public async Task<ResponseDto?> GradeStudentAnswerAsync(StudentQuizAnswerGradeDto gradeDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = gradeDto,
                Url = SD.CurriculumAPIBase + "/api/quizzes/studentanswer/grade"
            });
        }
        public async Task<ResponseDto?> GetStudentAnswerByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/quizzes/studentanswer/{id}"
            });
        }

        public async Task<ResponseDto?> GetTextBasedQuizzesWithPendingGradingAsync(string professorId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/quizzes/professor/{professorId}/textbased/pending"
            });
        }
    }
}