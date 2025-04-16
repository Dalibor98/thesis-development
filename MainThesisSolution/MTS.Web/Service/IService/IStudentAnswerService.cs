using MTS.Web.Models;
using MTS.Web.Models.Curriculum.Quiz.Used;

namespace MTS.Web.Service.IService
{
    public interface IStudentAnswerService
    {
        Task<ResponseDto?> GetAnswersByAttemptCodeAsync(string attemptCode);
        Task<ResponseDto?> GetAnswersByQuestionCodeAsync(string questionCode);
        Task<ResponseDto?> GetAnswersByStudentIdAsync(string studentId);
        Task<ResponseDto?> GetAnswerByIdAsync(int id);
        Task<ResponseDto?> CreateStudentAnswerAsync(StudentAnswerCreateDto answerDto);
        Task<ResponseDto?> UpdateStudentAnswerAsync(StudentAnswerUpdateDto answer);
        Task<ResponseDto?> GradeStudentAnswerAsync(StudentAnswerGradeDto gradeDto);
        Task<ResponseDto?> GetUngradedAnswersAsync(string professorId);
        Task<ResponseDto?> DeleteStudentAnswerAsync(int id);
    }
}
