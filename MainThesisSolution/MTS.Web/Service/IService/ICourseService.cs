using MTS.Web.Models;
using MTS.Web.Models.Curriculum.Course;

namespace MTS.Web.Service.IService
{
    public interface ICourseService
    {
        Task<ResponseDto?> GetAllCoursesAsync();
        Task<ResponseDto?> GetCourseByIdAsync(int id);
        Task<ResponseDto?> GetCourseByCodeAsync(string courseCode);
        Task<ResponseDto?> CreateCourseAsync(TemporaryCourseDTO courseDto);
        Task<ResponseDto?> UpdateCourseAsync(CourseUpdateDto courseDto);
        Task<ResponseDto?> DeleteCourseAsync(int id);
        Task<ResponseDto?> GetWeeksByCourseCodeAsync(string courseCode);
        Task<ResponseDto?> GetMaterialsByCourseCodeAsync(string courseCode);
        Task<ResponseDto?> GetAssignmentsByCourseCodeAsync(string courseCode);
        Task<ResponseDto?> GetQuizzesByCourseCodeAsync(string courseCode);

        Task<ResponseDto?> GetProfessorCoursesAsync(string professorId);
        Task<ResponseDto?> GetStudentCoursesAsync(string studentId);
        Task<ResponseDto?> GetRegistrationsByCourseCodeAsync(string courseCode);

    }
}