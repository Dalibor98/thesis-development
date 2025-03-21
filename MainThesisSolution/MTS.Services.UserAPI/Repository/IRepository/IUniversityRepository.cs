using MTS.Services.UserAPI.Models;

namespace MTS.Services.UserAPI.Repository.IRepository
{
    public interface IUniversityIdRepository
    {
        Task<IEnumerable<string>> GenerateUniversityIdsAsync(string type, int count);
        Task<bool> VerifyUniversityIdAsync(string code, string type);
        Task<bool> AssignUniversityIdAsync(string code);
        Task<IEnumerable<UniversityIdentifier>> GetAllUnassignedIdsAsync(string type);
    }
}
