using MTS.Services.UserAPI.Models;
using MTS.Services.UserAPI.Repository.IRepository;

namespace MTS.Services.UserAPI.Repository
{
    public class UniversityRepository : IUniversityIdRepository
    {
        public Task<bool> AssignUniversityIdAsync(string code)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GenerateUniversityIdsAsync(string type, int count)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UniversityIdentifier>> GetAllUnassignedIdsAsync(string type)
        {
            throw new NotImplementedException();
        }

        public Task<bool> VerifyUniversityIdAsync(string code, string type)
        {
            throw new NotImplementedException();
        }
    }
}
