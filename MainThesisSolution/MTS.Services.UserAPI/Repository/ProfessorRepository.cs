using Microsoft.EntityFrameworkCore;
using MTS.Services.UserAPI.Data;
using MTS.Services.UserAPI.Models;
using MTS.Services.UserAPI.Models.DTO;
using MTS.Services.UserAPI.Repository.IRepository;

namespace MTS.Services.UserAPI.Repository
{
    public class ProfessorRepository : IProfessorRepository
    {
        private readonly UserDbContext _db;
        private readonly IUniversityIdRepository _universityIdRepository;

        public ProfessorRepository(UserDbContext db, IUniversityIdRepository universityIdRepository)
        {
            _db = db;
            _universityIdRepository = universityIdRepository;
        }

        public async Task<IEnumerable<Professor>> GetAllProfessorsAsync()
        {
            return await _db.Professors.ToListAsync();
        }

        //here no validation for the same reasons as bellow, talk over this tomorrow!
        public async Task<Professor> GetProfessorByIdAsync(int id)
        {
            return await _db.Professors.FirstOrDefaultAsync(p => p.Id == id);
        }
        //we actually want to return null value here if we don't find the professor
        public async Task<Professor> GetProfessorByUniversityIdAsync(string universityId)
        {
            var temp = await _db.Professors.FirstOrDefaultAsync(p => p.UniversityId == universityId);
            return temp;
        }

        public async Task<Professor> CreateProfessorAsync(ProfessorCreateDto professorDto)
        {
            // First verify that the universityId is valid and of type PROFESSOR
            bool isValid = await _universityIdRepository.VerifyUniversityIdAsync(professorDto.UniversityId, "PROFESSOR");

            if (!isValid)
            {
                return null; // Invalid university ID
            }

            // Create new professor
            Professor professor = new Professor
            {
                Name = professorDto.Name,
                Email = professorDto.Email,
                UniversityId = professorDto.UniversityId,
                Department = professorDto.Department,
                Title = professorDto.Title,
            };

            _db.Professors.Add(professor);
            await _db.SaveChangesAsync();

            // Mark the university ID as assigned
            await _universityIdRepository.AssignUniversityIdAsync(professorDto.UniversityId);

            return professor;
        }

        public async Task<bool> UpdateProfessorAsync(int id, ProfessorCreateDto professorDto)
        {
            var professor = await _db.Professors.FirstOrDefaultAsync(p => p.Id == id);

            if (professor == null)
            {
                return false;
            }

            // Update professor properties except for universityId (which shouldn't change)
            professor.Name = professorDto.Name;
            professor.Email = professorDto.Email;
            professor.Department = professorDto.Department;
            professor.Title = professorDto.Title;

            _db.Professors.Update(professor);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteProfessorAsync(int id)
        {
            var professor = await _db.Professors.FirstOrDefaultAsync(p => p.Id == id);

            if (professor == null)
            {
                return false;
            }

            _db.Professors.Remove(professor);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}
