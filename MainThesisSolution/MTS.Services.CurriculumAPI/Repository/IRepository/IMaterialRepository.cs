using MTS.Services.CurriculumAPI.Models;

namespace MTS.Services.CurriculumAPI.Repository.IRepository
{
    public interface IMaterialRepository
    {
        Task<IEnumerable<Material>> GetAllMaterialsAsync();
        Task<Material?> GetMaterialByIdAsync(int id);
        Task<Material?> GetMaterialByCodeAsync(string materialCode);
        Task<IEnumerable<Material>> GetMaterialsByCourseCodeAsync(string courseCode);
        Task<IEnumerable<Material>> GetMaterialsByWeekCodeAsync(string weekCode);
        Task<Material> CreateMaterialAsync(Material material);
        Task<Material> UpdateMaterialAsync(Material material);
        Task<bool> DeleteMaterialAsync(int id);
    }
}
