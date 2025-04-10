using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using MTS.Services.CurriculumAPI.Data;
using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO;
using MTS.Services.CurriculumAPI.Repository.IRepository;
using MTS.Services.CurriculumAPI.Utilities;

namespace MTS.Services.CurriculumAPI.Repository
{
    public class MaterialRepository : IMaterialRepository
    {
        private readonly CurriculumDbContext _dbContext;

        public MaterialRepository(CurriculumDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<Material>> GetAllMaterialsAsync()
        {
            return await _dbContext.Materials.ToListAsync();
        }

        public async Task<Material?> GetMaterialByIdAsync(int id)
        {
            return await _dbContext.Materials.FindAsync(id);
        }

        public async Task<Material?> GetMaterialByCodeAsync(string materialCode)
        {
            return await _dbContext.Materials
                .FirstOrDefaultAsync(m => m.MaterialCode == materialCode);
        }

        public async Task<IEnumerable<Material>> GetMaterialsByCourseCodeAsync(string courseCode)
        {
            return await _dbContext.Materials
                .Where(m => m.CourseCode == courseCode)
                .ToListAsync();
        }

        public async Task<IEnumerable<Material>> GetMaterialsByWeekCodeAsync(string weekCode)
        {
            return await _dbContext.Materials
                .Where(m => m.WeekCode == weekCode)
                .ToListAsync();
        }

        public async Task<Material> CreateMaterialAsync(MaterialCreateDto materialDto)
        {
            var week = await _dbContext.Weeks.FirstOrDefaultAsync(w => w.WeekCode == materialDto.WeekCode);

            if (week == null)
            {
                throw new ArgumentNullException("Week with the given weekCode doesn't exist");
            }

            if (string.IsNullOrEmpty(materialDto.CourseCode))
            {
                materialDto.CourseCode = week.CourseCode;
            }
            var materialCode = await CodeGenerator.GenerateUniqueMaterialCode(_dbContext, week.WeekCode);
            // Map DTO to entity
            Material material = new Material
            {
                MaterialCode = materialCode,
                CourseCode = materialDto.CourseCode,
                WeekCode = materialDto.WeekCode,
                Title = materialDto.Title,
                Description = materialDto.Description,
                MaterialType = materialDto.MaterialType
                //FileUrl missing here? Handle it later.
            };

            _dbContext.Materials.Add(material);
            await _dbContext.SaveChangesAsync();
            return material;
        }

        public async Task<Material> UpdateMaterialAsync(MaterialUpdateDto materialDto)
        {
            var existingMaterial = await _dbContext.Materials.FirstOrDefaultAsync(m => m.MaterialCode== materialDto.MaterialCode);
            if (existingMaterial == null)
            {
                return null;
            }

            // Don't allow week code, or course code to be changed
            materialDto.CourseCode = existingMaterial.CourseCode;
            materialDto.WeekCode = existingMaterial.WeekCode;

            _dbContext.Entry(existingMaterial).CurrentValues.SetValues(materialDto);
            await _dbContext.SaveChangesAsync();
            return existingMaterial;
        }


        public async Task<bool> DeleteMaterialByCodeAsync(string materialCode)
        {
            var material = await _dbContext.Materials.FirstOrDefaultAsync(m => m.MaterialCode == materialCode);
            if (material == null)
            {
                return false;
            }

            _dbContext.Materials.Remove(material);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
