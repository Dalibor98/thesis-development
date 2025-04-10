using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO;

namespace MTS.Services.CurriculumAPI.Repository.IRepository
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<Course?> GetCourseByIdAsync(int id);
        Task<Course?> GetCourseByCodeAsync(string courseCode);
        Task<IEnumerable<Course>> GetCoursesByProfessorIdAsync(string professorUniversityId);
        Task<Course> CreateCourseAsync(TemporaryCourseDTO courseCreateDto);
        Task<Course> UpdateCourseAsync(CourseUpdateDto course);
        Task<bool> DeleteCourseAsync(int id);

        
        Task<IEnumerable<Week>> GetWeeksByCourseCodeAsync(string courseCode);
        Task<IEnumerable<Material>> GetMaterialsByCourseCodeAsync(string courseCode);
        Task<IEnumerable<Assignment>> GetAssignmentsByCourseCodeAsync(string courseCode);
        Task<IEnumerable<Quiz>> GetQuizzesByCourseCodeAsync(string courseCode);
        Task<IEnumerable<CourseRegistration>> GetRegistrationsByCourseCodeAsync(string courseCode);
    }
}
