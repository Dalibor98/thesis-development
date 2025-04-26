using Microsoft.EntityFrameworkCore;
using MTS.Services.CurriculumAPI.Models;

namespace MTS.Services.CurriculumAPI.Data
{
    public class CurriculumDbContext : DbContext
    {

        public DbSet<Course> Courses { get; set; }
        public DbSet<Week> Weeks { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<AnswerOption> AnswerOptions { get; set; }
        public DbSet<CourseRegistration> CourseRegistrations { get; set; }
        public DbSet<StudentQuizAttempt> StudentQuizAttempts { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }

        public CurriculumDbContext(DbContextOptions<CurriculumDbContext> options) : base(options)
        {
        }

    }
}
