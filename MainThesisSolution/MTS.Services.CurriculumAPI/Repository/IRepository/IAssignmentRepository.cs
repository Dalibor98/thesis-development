using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO;

namespace MTS.Services.CurriculumAPI.Repository.IRepository
{
    public interface IAssignmentRepository
    {
        Task<IEnumerable<Assignment>> GetAllAssignmentsAsync();
        Task<Assignment?> GetAssignmentByIdAsync(int id);
        Task<Assignment?> GetAssignmentByCodeAsync(string assignmentCode);
        Task<IEnumerable<Assignment>> GetAssignmentsByCourseCodeAsync(string courseCode);
        Task<IEnumerable<Assignment>> GetAssignmentsByWeekCodeAsync(string weekCode);
        Task<Assignment> CreateAssignmentAsync(AssignmentCreateDto assignment);
        Task<Assignment> UpdateAssignmentAsync(AssignmentUpdateDto assignment);
        Task<bool> DeleteAssignmentAsync(int id);
        Task<bool> DeleteAssignmentByCodeAsync(string assignmentCode);

        // Student submissions
        Task<IEnumerable<StudentAssignmentAttempt>> GetSubmissionsByAssignmentCodeAsync(string assignmentCode);
        Task<StudentAssignmentAttempt?> GetStudentSubmissionAsync(string assignmentCode, string studentUniversityId);
    }
}
