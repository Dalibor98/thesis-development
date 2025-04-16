using MTS.Services.CurriculumAPI.Models.DTO.QuizDto;
using MTS.Services.CurriculumAPI.Models;

namespace MTS.Services.CurriculumAPI.Repository.IRepository
{
    public interface IQuizQuestionRepository
    {
        Task<IEnumerable<QuizQuestion>> GetQuestionsByQuizCodeAsync(string quizCode);
        Task<QuizQuestion?> GetQuestionByCodeAsync(string questionCode);
        Task<QuizQuestion> CreateQuestionAsync(QuizQuestionCreateDto question);
        Task<QuizQuestion> UpdateQuestionAsync(QuizQuestionUpdateDto question);
        Task<bool> DeleteQuestionByCodeAsync(string questionCode);
    }
}
