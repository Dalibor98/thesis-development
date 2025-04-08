using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO;

namespace MTS.Services.CurriculumAPI.Repository.IRepository
{
    public interface IWeekRepository
    {
        Task<IEnumerable<Week>> GetAllWeeksAsync();
        Task<Week?> GetWeekByIdAsync(int id);
        Task<Week?> GetWeekByCodeAsync(string weekCode);
        Task<IEnumerable<Week>> GetWeeksByCourseCodeAsync(string courseCode);
        Task<Week> CreateWeekAsync(WeekCreateDto week);
        Task<Week> UpdateWeekAsync(WeekUpdateDto week);
        Task<bool> DeleteWeekAsync(int id);

        // Related data
        Task<IEnumerable<Material>> GetMaterialsByWeekCodeAsync(string weekCode);
        Task<IEnumerable<Assignment>> GetAssignmentsByWeekCodeAsync(string weekCode);
        Task<IEnumerable<Quiz>> GetQuizzesByWeekCodeAsync(string weekCode);
    }
}
