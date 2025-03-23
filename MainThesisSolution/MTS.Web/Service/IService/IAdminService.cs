using MTS.Web.Models;
using System.Threading.Tasks;

namespace MTS.Web.Service.IService
{
    public interface IAdminService
    {
        Task<ResponseDto?> GetStudentsAsync();
        Task<ResponseDto?> GetStudentByIdAsync(int id);
        Task<ResponseDto?> GenerateIds(UniversityIdGenerateDto universityIdGenerateDto);
        Task<ResponseDto?> VerifyId(UniversityIdVerifyDto universityIdGenerateDto);
        Task<ResponseDto?> GetUnassignedIds(string type);

        Task<ResponseDto?> UpdateStudentAsync(StudentDto studentDto);
        Task<ResponseDto?> GetProfessorsAsync();
    }
}
