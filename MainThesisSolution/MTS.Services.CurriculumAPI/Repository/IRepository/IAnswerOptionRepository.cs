using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO.AnswerOption;

namespace MTS.Services.CurriculumAPI.Repository.IRepository
{
    public interface IAnswerOptionRepository
    {
        Task<IEnumerable<AnswerOption>> GetOptionsByQuestionCodeAsync(string questionCode);
        Task<AnswerOption?> GetOptionByCodeAsync(string optionCode);
        Task<AnswerOption?> GetOptionByIdAsync(int id);

        // Create new answer option
        Task<AnswerOption> CreateOptionAsync(AnswerOptionCreateDto optionDto);

        // Update existing answer option
        Task<AnswerOption> UpdateOptionAsync(AnswerOptionUpdateDto optionDto);

        // Delete answer option
        Task<bool> DeleteOptionAsync(int id);
        Task<bool> DeleteOptionByCodeAsync(string optionCode);

        // Get correct option(s) for a question
        Task<IEnumerable<AnswerOption>> GetCorrectOptionsForQuestionAsync(string questionCode);
    }
}