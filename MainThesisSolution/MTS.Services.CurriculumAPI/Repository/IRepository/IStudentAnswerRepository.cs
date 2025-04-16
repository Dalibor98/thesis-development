using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO.StudentAnswer;
using MTS.Services.CurriculumAPI.Models.DTO.StudentAnswerDto;

namespace MTS.Services.CurriculumAPI.Repository.IRepository
{
    public interface IStudentAnswerRepository
    {
        Task<IEnumerable<StudentAnswer>> GetAnswersByAttemptCodeAsync(string attemptCode);
        Task<IEnumerable<StudentAnswer>> GetAnswersByQuestionCodeAsync(string questionCode);
        Task<IEnumerable<StudentAnswer>> GetAnswersByStudentIdAsync(string studentUniversityId);
        Task<StudentAnswer?> GetAnswerByIdAsync(int id);
        Task<StudentAnswer> CreateStudentAnswerAsync(StudentAnswerCreateDto answerDto);
        Task<StudentAnswer> UpdateStudentAnswerAsync(StudentAnswer answer);
        Task<StudentAnswer> GradeStudentAnswerAsync(StudentAnswerGradeDto gradeDto);
        Task<IEnumerable<StudentAnswer>> GetUngradedAnswersAsync(string professorId);
        Task<bool> DeleteStudentAnswerAsync(int id);
    }
}