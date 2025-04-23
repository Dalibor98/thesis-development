using MTS.Services.CurriculumAPI.Models.DTO.QuizDto;
using MTS.Services.CurriculumAPI.Models;

namespace MTS.Services.CurriculumAPI.Repository.IRepository
{
    public interface IStudentQuizAttemptRepository
    {
        Task<IEnumerable<StudentQuizAttempt>> GetAllAttemptsAsync();
        Task<IEnumerable<StudentQuizAttempt>> GetAttemptsByQuizCodeAsync(string quizCode);
        Task<IEnumerable<StudentQuizAttempt>> GetAttemptsByStudentIdAsync(string studentUniversityId);
        Task<StudentQuizAttempt?> GetAttemptByCodeAsync(string attemptCode);
        Task<StudentQuizAttempt> CreateAttemptAsync(StudentQuizAttemptCreateDto attempt);
        Task<StudentQuizAttempt> UpdateAttemptAsync(StudentQuizAttempt attempt);
        Task<IEnumerable<StudentQuizAttempt>> GetRecentAttemptsByProfessorIdAsync(string professorId);
        Task<int> CalculateAndUpdateScoreAsync(string attemptCode);
    }
}
