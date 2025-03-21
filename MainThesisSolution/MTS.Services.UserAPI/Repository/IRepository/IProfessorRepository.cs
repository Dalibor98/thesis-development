using MTS.Services.UserAPI.Models;
using MTS.Services.UserAPI.Models.DTO;

namespace MTS.Services.UserAPI.Repository.IRepository
{
    public interface IProfessorRepository
    {
        Task<IEnumerable<Professor>> GetAllProfessorsAsync();
        Task<Professor> GetProfessorByIdAsync(string id);
        Task<Professor> GetProfessorByUniversityIdAsync(string universityId);
        Task<Professor> CreateProfessorAsync(ProfessorCreateDto professorDto);
        Task<bool> UpdateProfessorAsync(string id, ProfessorCreateDto professorDto);
        Task<bool> DeleteProfessorAsync(string id);
    }
}
