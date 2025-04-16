using MTS.Web.Models;
using MTS.Web.Models.Curriculum.Quiz.Used;

namespace MTS.Web.Service.IService
{
    public interface IQuizQuestionService
    {
        Task<ResponseDto?> GetQuestionsByQuizCodeAsync(string quizCode);
        Task<ResponseDto?> GetQuestionByCodeAsync(string questionCode);
        Task<ResponseDto?> CreateQuestionAsync(QuizQuestionCreateDto questionDto);
        Task<ResponseDto?> UpdateQuestionAsync(QuizQuestionUpdateDto questionDto);
        Task<ResponseDto?> DeleteQuestionAsync(string questionCode);
    }
}
