using Microsoft.EntityFrameworkCore;
using MTS.Services.CurriculumAPI.Data;
using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO;
using MTS.Services.CurriculumAPI.Repository.IRepository;

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
            // Generate material code if not provided
            if (string.IsNullOrEmpty(materialDto.MaterialCode))
            {
                materialDto.MaterialCode = Material.GenerateMaterialCode(materialDto.WeekCode);
            }

            // Ensure course code is set if we have a week code
            if (!string.IsNullOrEmpty(materialDto.WeekCode) && string.IsNullOrEmpty(materialDto.CourseCode))
            {
                var week = await _dbContext.Weeks
                    .FirstOrDefaultAsync(w => w.WeekCode == materialDto.WeekCode);
                if (week != null)
                {
                    materialDto.CourseCode = week.CourseCode;
                }
            }

            // Map DTO to entity
            Material material = new Material
            {
                MaterialCode = materialDto.MaterialCode,
                CourseCode = materialDto.CourseCode,
                WeekCode = materialDto.WeekCode,
                Title = materialDto.Title,
                Description = materialDto.Description,
                MaterialType = materialDto.MaterialType
                // FileUrl property is commented out in the DTO
                // If you need to include it, uncomment and add: FileUrl = materialDto.FileUrl
            };

            _dbContext.Materials.Add(material);
            await _dbContext.SaveChangesAsync();
            return material;
        }

        public async Task<Material> UpdateMaterialAsync(MaterialCreateDto materialDto)
        {
            var existingMaterial = await _dbContext.Materials.FirstOrDefaultAsync(m => m.CourseCode == materialDto.CourseCode);
            if (existingMaterial == null)
            {
                return null;
            }

            // Don't allow course code, week code, or material code to be changed
            materialDto.CourseCode = existingMaterial.CourseCode;
            materialDto.WeekCode = existingMaterial.WeekCode;
            materialDto.MaterialCode = existingMaterial.MaterialCode;

            _dbContext.Entry(existingMaterial).CurrentValues.SetValues(materialDto);
            await _dbContext.SaveChangesAsync();
            return existingMaterial;
        }

        public async Task<bool> DeleteMaterialAsync(int id)
        {
            var material = await _dbContext.Materials.FindAsync(id);
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
