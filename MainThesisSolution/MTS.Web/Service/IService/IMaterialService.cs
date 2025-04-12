using MTS.Web.Models;
using MTS.Web.Models.Curriculum.Material;

namespace MTS.Web.Service.IService
{
    public interface IMaterialService
    {
        Task<ResponseDto?> GetMaterialByCodeAsync(string materialCode);
        Task<ResponseDto?> GetMaterialsByWeekCodeAsync(string weekCode);
        Task<ResponseDto?> GetMaterialsByCourseCodeAsync(string courseCode);
        Task<ResponseDto?> CreateMaterialAsync(MaterialCreateDto materialDto);
        Task<ResponseDto?> UpdateMaterialAsync(MaterialCreateDto materialDto);
        Task<ResponseDto?> DeleteMaterialAsync(string materialCode);
    }
}
