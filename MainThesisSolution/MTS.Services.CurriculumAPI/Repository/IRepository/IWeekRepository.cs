using MTS.Services.CurriculumAPI.Models;

namespace MTS.Services.CurriculumAPI.Repository.IRepository
{
    public interface IWeekRepository
    {
        Task<IEnumerable<Week>> GetAllWeeksAsync();
        Task<Week?> GetWeekByIdAsync(int id);
        Task<Week?> GetWeekByCodeAsync(string weekCode);
        Task<IEnumerable<Week>> GetWeeksByCourseCodeAsync(string courseCode);
        Task<Week> CreateWeekAsync(Week week);
        Task<Week> UpdateWeekAsync(Week week);
        Task<bool> DeleteWeekAsync(int id);

        // Related data
        Task<IEnumerable<Material>> GetMaterialsByWeekCodeAsync(string weekCode);
        Task<IEnumerable<Assignment>> GetAssignmentsByWeekCodeAsync(string weekCode);
        Task<IEnumerable<Quiz>> GetQuizzesByWeekCodeAsync(string weekCode);
    }
}
