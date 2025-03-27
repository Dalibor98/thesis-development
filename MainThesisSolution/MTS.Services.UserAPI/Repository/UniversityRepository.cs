using Microsoft.EntityFrameworkCore;
using MTS.Services.UserAPI.Data;
using MTS.Services.UserAPI.Models;
using MTS.Services.UserAPI.Repository.IRepository;
using MTS.Services.UserAPI.Utility;

namespace MTS.Services.UserAPI.Repository
{
    public class UniversityIdRepository : IUniversityIdRepository
    {
        private readonly UserDbContext _db;
        private readonly Random _random = new Random();

        public UniversityIdRepository(UserDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<string>> GenerateUniversityIdsAsync(string type, int count)
        {
            if (type != "STUDENT" && type != "PROFESSOR")
            {
                throw new ArgumentException("Type must be either STUDENT or PROFESSOR");
            }

            List<string> generatedIds = new List<string>();

            var prefix = type == "STUDENT" ? "S" : "P";

            var codeType = type == "STUDENT" ? IdType.STUDENT : IdType.PROFESSOR;

            for (int i = 0; i < count; i++)
            {
                // Generate a unique 8-digit code with prefix (S for students, P for professors)
                // Followed by the current year (last two digits), and a random 6-digit number.
                string code;
                do
                {
                    code = prefix + DateTime.Now.Year.ToString().Substring(2) + _random.Next(100000, 999999).ToString();
                } while (await _db.UniversityIdentifiers.AnyAsync(u => u.Code == code));

                // Create and save the university ID
                UniversityIdentifier universityId = new UniversityIdentifier
                {
                    Code = code,
                    Type = codeType,
                    IsAssigned = false,
                };

                _db.UniversityIdentifiers.Add(universityId);
                generatedIds.Add(code);
            }

            await _db.SaveChangesAsync();
            return generatedIds;
        }

        public async Task<bool> VerifyUniversityIdAsync(string code, string type)
        {
            var codeType = type == "STUDENT" ? IdType.STUDENT : IdType.PROFESSOR;
            // Check if the university ID exists, is of the correct type, and is not already assigned
            var universityId = await _db.UniversityIdentifiers.FirstOrDefaultAsync(
                u => u.Code == code && u.Type == codeType && !u.IsAssigned);

            return universityId != null;
        }

        public async Task<bool> AssignUniversityIdAsync(string code)
        {
            var universityId = await _db.UniversityIdentifiers.FirstOrDefaultAsync(u => u.Code == code && !u.IsAssigned);

            if (universityId == null)
            {
                return false;
            }

            universityId.IsAssigned = true;

            await _db.SaveChangesAsync();
            return true;
        }
        //Upon creation of student or professor they have the right validation, This is not neccesarry but its commented for now.
        /*
        public async Task<bool> IsStudentAsync(string universityIdCode)
        {
            if (!universityIdCode.StartsWith('S'))
            {
                return false;
            }

            return await VerifyUniversityIdAsync(universityIdCode, "STUDENT");
        }
        */
        public async Task<IEnumerable<UniversityIdentifier>> GetAllUnassignedIdsAsync(string type)
        {
            var codeType = type == "STUDENT" ? IdType.STUDENT : IdType.PROFESSOR;

            IEnumerable<UniversityIdentifier> codes = await _db.UniversityIdentifiers
                .Where(u => u.Type == codeType && !u.IsAssigned)
                .ToListAsync();

            if (codes.Any())
            {
                return codes;
            }
            else
            {
                await GenerateUniversityIdsAsync(type, 1);

                return await _db.UniversityIdentifiers
                    .Where(u => u.Type == codeType && !u.IsAssigned)
                    .ToListAsync();
            }
        }
    }
}
