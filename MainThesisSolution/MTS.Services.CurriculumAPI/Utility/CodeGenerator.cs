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
        public static async Task<string> GenerateUniqueAnswerCode(CurriculumDbContext dbContext, string questionCode)
        {
            string code;
            int answerNumber = 1;
            do
            {
                code = $"{questionCode}-A{answerNumber:D2}";
                answerNumber++;
            }
            while (await dbContext.Answers.AnyAsync(a => a.AnswerCode == code));
            return code;
        }

        
        public static async Task<string> GenerateUniqueQuestionCode(CurriculumDbContext dbContext, string quizCode)
        {
            string code;
            int questionNumber = 1;
            do
            {
                code = $"{quizCode}-Q{questionNumber:D2}";
                questionNumber++;
            }
            while (await dbContext.QuizQuestions.AnyAsync(q => q.QuizQuestionCode == code));
            return code;
        }

        
        public static async Task<string> GenerateUniqueAttemptCode(CurriculumDbContext dbContext, string quizCode, string studentId)
        {
            string code;
            do
            {
                code = $"{quizCode}-{studentId}-{DateTime.Now.Ticks}";
            }
            while (await dbContext.StudentQuizAttempts.AnyAsync(a => a.AttemptCode == code));
            return code;
        }
    }
}
