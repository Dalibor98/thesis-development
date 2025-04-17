using MTS.Web.Models;
using MTS.Web.Models.Curriculum.Quiz;

namespace MTS.Web.Service.IService
{
    public interface IAnswerOptionService
    {
        Task<ResponseDto?> GetOptionsByQuestionCodeAsync(string questionCode);
        Task<ResponseDto?> GetOptionByCodeAsync(string optionCode);
        Task<ResponseDto?> CreateOptionAsync(AnswerOptionCreateDto optionDto);
        Task<ResponseDto?> UpdateOptionAsync(AnswerOptionUpdateDto optionDto);
        Task<ResponseDto?> DeleteOptionAsync(int id);
        Task<ResponseDto?> DeleteOptionByCodeAsync(string optionCode);
        Task<ResponseDto?> GetCorrectOptionsForQuestionAsync(string questionCode);
    }
}
