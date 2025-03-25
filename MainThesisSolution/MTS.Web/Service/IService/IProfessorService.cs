using MTS.Web.Models;

namespace MTS.Web.Service.IService
{
    public interface IProfessorService
    {
        Task<ResponseDto?> GetAllProfessorsAsync();
        Task<ResponseDto?> GetProfessorByIdAsync(int id);
        Task<ResponseDto?> CreateProfessorAsync(ProfessorCreateDto professorDto);
        Task<ResponseDto?> UpdateProfessorAsync(int id, ProfessorCreateDto professorDto);
        Task<ResponseDto?> DeleteProfessorAsync(int id);
    }
}