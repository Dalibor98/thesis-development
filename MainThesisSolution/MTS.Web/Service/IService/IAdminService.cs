using MTS.Web.Models;

namespace MTS.Web.Service.IService
{
    public interface IAdminService
    {
        Task<ResponseDto?> GenerateIds(UniversityIdGenerateDto universityIdGenerateDto);
        Task<ResponseDto?> VerifyId(UniversityIdVerifyDto universityIdGenerateDto);
        Task<ResponseDto?> GetUnassignedIds(string type);
    }
}
