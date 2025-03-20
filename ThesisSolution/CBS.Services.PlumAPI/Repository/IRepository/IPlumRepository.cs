using CBS.Services.PlumAPI.Models;

namespace CBS.Services.PlumAPI.Repository.IRepository
{
    public interface IPlumRepository
    {
        Task<IEnumerable<Plum>> GetAllPlumsAsync();
        Task<Plum> GetPlumByIdAsync(int id);
        Task<Plum> CreatePlumAsync(Plum plum);
        Task<Plum> UpdatePlumAsync(Plum plum);
        Task<bool> DeletePlumAsync(int id);
    }
}
