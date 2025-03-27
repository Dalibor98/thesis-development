using MTS.Web.Models;

namespace MTS.Web.Service.IService
{
    public interface IUniversityIdService
    {
        Task<ResponseDto?> GenerateUniversityIdsAsync(string type, int count);
        Task<ResponseDto?> VerifyUniversityIdAsync(string code, string type);
        Task<ResponseDto?> GetUnassignedIdsAsync(string type);
    }
}