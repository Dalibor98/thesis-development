using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO;

namespace MTS.Services.CurriculumAPI.Repository.IRepository
{
    public interface IMaterialRepository
    {
        Task<IEnumerable<Material>> GetAllMaterialsAsync();
        Task<Material?> GetMaterialByIdAsync(int id);
        Task<Material?> GetMaterialByCodeAsync(string materialCode);
        Task<IEnumerable<Material>> GetMaterialsByCourseCodeAsync(string courseCode);
        Task<IEnumerable<Material>> GetMaterialsByWeekCodeAsync(string weekCode);
        Task<Material> CreateMaterialAsync(MaterialCreateDto material);
        Task<Material> UpdateMaterialAsync(MaterialUpdateDto material);
        Task<bool> DeleteMaterialByCodeAsync(string materialCode);
    }
}
