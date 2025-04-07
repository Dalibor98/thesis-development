using MTS.Services.CurriculumAPI.Models.DTO;
using MTS.Services.CurriculumAPI.Models;

namespace MTS.Services.CurriculumAPI.Repository.IRepository
{
    public interface IStudentAssignmentAttemptRepository
    {
        Task<StudentAssignmentAttempt> CreateAttemptAsync(StudentAssignmentAttemptCreateDto attemptDto);
        Task<StudentAssignmentAttempt> UpdateAttemptAsync(StudentAssignmentAttemptUpdateDto attemptDto);
        Task<IEnumerable<StudentAssignmentAttempt>> GetAttemptsByStudentIdAsync(string studentUniversityId);
        Task<IEnumerable<StudentAssignmentAttempt>> GetAttemptsByAssignmentCodeAsync(string assignmentCode);
        Task<StudentAssignmentAttempt> GetAttemptByIdAsync(int id);
        Task<StudentAssignmentAttempt> GetAttemptByStudentAndAssignmentAsync(string studentUniversityId, string assignmentCode);
    }
}