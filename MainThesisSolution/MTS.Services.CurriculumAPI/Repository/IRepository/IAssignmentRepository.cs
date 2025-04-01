using MTS.Services.CurriculumAPI.Models;

namespace MTS.Services.CurriculumAPI.Repository.IRepository
{
    public interface IAssignmentRepository
    {
        Task<IEnumerable<Assignment>> GetAllAssignmentsAsync();
        Task<Assignment?> GetAssignmentByIdAsync(int id);
        Task<Assignment?> GetAssignmentByCodeAsync(string assignmentCode);
        Task<IEnumerable<Assignment>> GetAssignmentsByCourseCodeAsync(string courseCode);
        Task<IEnumerable<Assignment>> GetAssignmentsByWeekCodeAsync(string weekCode);
        Task<Assignment> CreateAssignmentAsync(Assignment assignment);
        Task<Assignment> UpdateAssignmentAsync(Assignment assignment);
        Task<bool> DeleteAssignmentAsync(int id);

        // Student submissions
        Task<IEnumerable<StudentAssignmentAttempt>> GetSubmissionsByAssignmentCodeAsync(string assignmentCode);
        Task<StudentAssignmentAttempt?> GetStudentSubmissionAsync(string assignmentCode, string studentUniversityId);
    }
}
