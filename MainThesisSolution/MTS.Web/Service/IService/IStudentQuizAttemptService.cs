using MTS.Web.Models;
using MTS.Web.Models.Curriculum.Quiz;

namespace MTS.Web.Service.IService
{
    public interface IStudentQuizAttemptService
    {
        Task<ResponseDto?> GetAttemptsByQuizCodeAsync(string quizCode);
        Task<ResponseDto?> GetAttemptsByStudentIdAsync(string studentUniversityId);
        Task<ResponseDto?> GetAttemptByCodeAsync(string attemptCode);
        Task<ResponseDto?> CreateAttemptAsync(StudentQuizAttemptCreateDto attempt);
        Task<ResponseDto?> UpdateAttemptAsync(StudentQuizAttemptUpdateDto attempt);
        Task<ResponseDto?> GetRecentAttemptsByProfessorIdAsync(string professorId);
        Task<ResponseDto?> CalculateAndUpdateScoreAsync(string attemptCode);
    }
}
