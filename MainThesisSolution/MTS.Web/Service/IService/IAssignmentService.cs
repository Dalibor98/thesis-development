using MTS.Web.Models;
using MTS.Web.Models.Curriculum.Assignment;

namespace MTS.Web.Service.IService
{
    public interface IAssignmentService
    {
        Task<ResponseDto?> GetAssignmentByCodeAsync(string assignmentCode);
        Task<ResponseDto?> GetSubmissionsByAssignmentCodeAsync(string assignmentCode);
        Task<ResponseDto?> GetStudentSubmissionAsync(string assignmentCode, string studentUniversityId);
        Task<ResponseDto?> SubmitAssignmentAsync(StudentAssignmentAttemptCreateDto submissionDto);
        Task<ResponseDto?> GradeAssignmentAsync(StudentAssignmentGradeDto gradeDto);
        Task<ResponseDto?> CreateAssignmentAsync(AssignmentCreateDto assignmentDto);
        Task<ResponseDto?> UpdateAssignmentAsync(AssignmentUpdateDto assignmentDto);
        Task<ResponseDto?> DeleteAssignmentAsync(string assignmentCode);
        Task<ResponseDto?> GetUpcomingAssignmentsByStudentIdAsync(string studentId);
        Task<ResponseDto?> GetRecentSubmissionsByProfessorIdAsync(string professorId);
    }
}