using MTS.Web.Models;
using MTS.Web.Models.User.Student;

namespace MTS.Web.Service.IService
{
    public interface IStudentService
    {
        Task<ResponseDto?> GetAllStudentsAsync();
        Task<ResponseDto?> GetStudentByIdAsync(int id);
        Task<ResponseDto?> CreateStudentAsync(StudentCreateDto studentDto);
        Task<ResponseDto?> UpdateStudentAsync(int id, StudentCreateDto studentDto);
        Task<ResponseDto?> DeleteStudentAsync(int id);
    }
}