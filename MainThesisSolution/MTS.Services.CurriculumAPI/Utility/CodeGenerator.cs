using Microsoft.EntityFrameworkCore;
using MTS.Services.CurriculumAPI.Data;

namespace MTS.Services.CurriculumAPI.Utilities
{
    public static class CodeGenerator
    {
        public static async Task<string> GenerateUniqueCourseCode(CurriculumDbContext dbContext)
        {
            string code;
            do
            {
                code = $"CRS-{Guid.NewGuid().ToString().Substring(0, 8)}";
            }
            while (await dbContext.Courses.AnyAsync(c => c.CourseCode == code));

            return code;
        }

        public static async Task<string> GenerateUniqueAssignmentCode(CurriculumDbContext dbContext, string weekCode)
        {
            string code;
            do
            {
                code = $"{weekCode}-ASN-{Guid.NewGuid().ToString().Substring(0, 6)}";
            }
            while (await dbContext.Assignments.AnyAsync(a => a.AssignmentCode == code));

            return code;
        }

        public static async Task<string> GenerateUniqueMaterialCode(CurriculumDbContext dbContext, string weekCode)
        {
            string code;
            do
            {
                code = $"{weekCode}-MAT-{Guid.NewGuid().ToString().Substring(0, 6)}";
            }
            while (await dbContext.Materials.AnyAsync(m => m.MaterialCode == code));

            return code;
        }

        public static async Task<string> GenerateUniqueQuizCode(CurriculumDbContext dbContext, string weekCode)
        {
            string code;
            do
            {
                code = $"{weekCode}-QZ-{Guid.NewGuid().ToString().Substring(0, 6)}";
            }
            while (await dbContext.Quizzes.AnyAsync(q => q.QuizCode == code));

            return code;
        }
        public static async Task<string> GenerateUniqueWeekCode(CurriculumDbContext dbContext, string courseCode, int weekNumber)
        {
            string code;
            do
            {
                code = $"{courseCode}-W{weekNumber:D2}";
            }
            while (await dbContext.Weeks.AnyAsync(w => w.WeekCode == code));

            return code;
        }
    }
}
