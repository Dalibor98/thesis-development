using MTS.Web.Models;

namespace MTS.Web.Service.IService
{
    public interface IAssignmentService
    {
        Task<ResponseDto?> GetAssignmentByCodeAsync(string assignmentCode);
        Task<ResponseDto?> GetSubmissionsByAssignmentCodeAsync(string assignmentCode);
        Task<ResponseDto?> GetStudentSubmissionAsync(string assignmentCode, string studentUniversityId);
        Task<ResponseDto?> SubmitAssignmentAsync(StudentAssignmentAttemptCreateDto submissionDto);
        Task<ResponseDto?> GradeAssignmentAsync(StudentAssignmentGradeDto gradeDto);
    }
}
