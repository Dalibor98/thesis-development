using MTS.Services.UserAPI.Models;
using MTS.Services.UserAPI.Models.DTO;

namespace MTS.Services.UserAPI.Repository.IRepository
{
    public interface IProfessorRepository
    {
        Task<IEnumerable<Professor>> GetAllProfessorsAsync();
        Task<Professor> GetProfessorByIdAsync(int id);
        Task<Professor> GetProfessorByUniversityIdAsync(string universityId);
        Task<Professor> CreateProfessorAsync(ProfessorCreateDto professorDto);
        Task<bool> UpdateProfessorAsync(int id, ProfessorCreateDto professorDto);
        Task<bool> DeleteProfessorAsync(int id);
    }
}
