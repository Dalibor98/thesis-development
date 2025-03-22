using MTS.Services.UserAPI.Models;
using MTS.Services.UserAPI.Models.DTO;

namespace MTS.Services.UserAPI.Repository.IRepository
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<Student> GetStudentByIdAsync(int id);
        Task<Student> GetStudentByUniversityIdAsync(string universityId);
        Task<Student> CreateStudentAsync(StudentCreateDto studentDto);
        Task<bool> UpdateStudentAsync(int id, StudentCreateDto studentDto);
        Task<bool> DeleteStudentAsync(int id);
    }
}
