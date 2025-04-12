using MTS.Web.Models;
using MTS.Web.Models.User.Student;
using MTS.Web.Models.User.UniId;
using System.Threading.Tasks;

namespace MTS.Web.Service.IService
{
    public interface IAdminService
    {
        Task<ResponseDto?> GetStudentsAsync();
        Task<ResponseDto?> GetStudentByIdAsync(int id);
        Task<ResponseDto?> GenerateIds(UniversityIdGenerateDto universityIdGenerateDto);
        Task<ResponseDto?> VerifyId(UniversityIdVerifyDto verifyDto);
        Task<ResponseDto?> GetUnassignedIds(string type);

        Task<ResponseDto?> UpdateStudentAsync(StudentDto studentDto);
        Task<ResponseDto?> GetProfessorsAsync();
        Task<ResponseDto?> DeleteStudentAsync(int id);

    }
}
