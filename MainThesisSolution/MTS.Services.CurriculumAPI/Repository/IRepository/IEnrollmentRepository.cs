using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO;

namespace MTS.Services.CurriculumAPI.Repository.IRepository
{
    public interface IEnrollmentRepository
    {
        Task<IEnumerable<CourseRegistration>> GetAllEnrollmentsAsync();
        Task<IEnumerable<CourseRegistration>> GetEnrollmentsByStudentIdAsync(string studentUniversityId);
        Task<IEnumerable<CourseRegistration>> GetEnrollmentsByCourseCodeAsync(string courseCode);
        Task<CourseRegistration> GetEnrollmentAsync(string courseCode, string studentUniversityId);
        Task<CourseRegistration> CreateEnrollmentAsync(EnrollmentCreateDto enrollmentDto);
        Task<CourseRegistration> UpdateEnrollmentAsync(EnrollmentUpdateDto enrollmentDto);
        Task<bool> DeleteEnrollmentAsync(int id);
        Task<bool> IsStudentEnrolledAsync(string courseCode, string studentUniversityId);
    }
}
