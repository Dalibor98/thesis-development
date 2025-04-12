using MTS.Web.Models;
using MTS.Web.Models.User.Professor;

namespace MTS.Web.Service.IService
{
    public interface IProfessorService
    {
        Task<ResponseDto?> GetAllProfessorsAsync();
        Task<ResponseDto?> GetProfessorByIdAsync(int id);
        Task<ResponseDto?> CreateProfessorAsync(ProfessorCreateDto professorDto);
        Task<ResponseDto?> UpdateProfessorAsync(ProfessorDto professorDto);
        Task<ResponseDto?> DeleteProfessorAsync(int id);
    }
}