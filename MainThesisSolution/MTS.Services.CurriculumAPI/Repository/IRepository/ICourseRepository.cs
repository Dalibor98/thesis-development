using MTS.Services.CurriculumAPI.Models;

namespace MTS.Services.CurriculumAPI.Repository.IRepository
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<Course?> GetCourseByIdAsync(int id);
        Task<Course?> GetCourseByCodeAsync(string courseCode);
        Task<IEnumerable<Course>> GetCoursesByProfessorIdAsync(string professorUniversityId);
        Task<Course> CreateCourseAsync(Course course);
        Task<Course> UpdateCourseAsync(Course course);
        Task<bool> DeleteCourseAsync(int id);

        
        Task<IEnumerable<Week>> GetWeeksByCourseCodeAsync(string courseCode);
        Task<IEnumerable<Material>> GetMaterialsByCourseCodeAsync(string courseCode);
        Task<IEnumerable<Assignment>> GetAssignmentsByCourseCodeAsync(string courseCode);
        Task<IEnumerable<Quiz>> GetQuizzesByCourseCodeAsync(string courseCode);
        Task<IEnumerable<CourseRegistration>> GetRegistrationsByCourseCodeAsync(string courseCode);
    }
}
