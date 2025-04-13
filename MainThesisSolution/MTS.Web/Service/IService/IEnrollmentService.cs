using MTS.Web.Models;

namespace MTS.Web.Service.IService
{
    public interface IEnrollmentService
    {
        Task<ResponseDto?> GetStudentEnrollmentsAsync(string studentUniversityId);
        Task<ResponseDto?> GetCourseEnrollmentsAsync(string courseCode);
        Task<ResponseDto?> EnrollStudentAsync(string courseCode, string studentUniversityId);
        Task<ResponseDto?> DropCourseAsync(int enrollmentId);
        Task<ResponseDto?> IsStudentEnrolledAsync(string courseCode, string studentUniversityId);
    }
}
