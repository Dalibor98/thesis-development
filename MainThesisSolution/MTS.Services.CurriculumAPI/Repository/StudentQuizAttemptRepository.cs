using Microsoft.EntityFrameworkCore;
using MTS.Services.CurriculumAPI.Data;
using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO.QuizDto;
using MTS.Services.CurriculumAPI.Repository.IRepository;
using MTS.Services.CurriculumAPI.Utilities;

namespace MTS.Services.CurriculumAPI.Repository
{
    public class StudentQuizAttemptRepository : IStudentQuizAttemptRepository
    {
        private readonly CurriculumDbContext _dbContext;
        private readonly IStudentAnswerRepository _studentAnswerRepository;

        public StudentQuizAttemptRepository(CurriculumDbContext dbContext,
                                            IStudentAnswerRepository studentAnswerRepository)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _studentAnswerRepository = studentAnswerRepository ??
                                      throw new ArgumentNullException(nameof(studentAnswerRepository));
        }

        public async Task<IEnumerable<StudentQuizAttempt>> GetAttemptsByQuizCodeAsync(string quizCode)
        {
            return await _dbContext.StudentQuizAttempts
                .Where(a => a.QuizCode == quizCode)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentQuizAttempt>> GetAttemptsByStudentIdAsync(string studentUniversityId)
        {
            return await _dbContext.StudentQuizAttempts
                .Where(a => a.StudentUniversityId == studentUniversityId)
                .ToListAsync();
        }

        public async Task<StudentQuizAttempt?> GetAttemptByCodeAsync(string attemptCode)
        {
            return await _dbContext.StudentQuizAttempts
                .FirstOrDefaultAsync(a => a.AttemptCode == attemptCode);
        }

        public async Task<StudentQuizAttempt> CreateAttemptAsync(StudentQuizAttemptCreateDto attemptDto)
        {
            // Validate that the quiz exists
            var quiz = await _dbContext.Quizzes
                .FirstOrDefaultAsync(q => q.QuizCode == attemptDto.QuizCode);

            if (quiz == null)
            {
                throw new ArgumentException($"Quiz with code {attemptDto.QuizCode} not found");
            }

            // Generate a unique attempt code
            string attemptCode = await CodeGenerator.GenerateUniqueAttemptCode(
                _dbContext, attemptDto.QuizCode, attemptDto.StudentUniversityId);

            var attempt = new StudentQuizAttempt
            {
                AttemptCode = attemptCode,
                EndTime = attemptDto.EndTime,
                StartTime = attemptDto.StartTime,
                Score = attemptDto.Score,
                StudentUniversityId = attemptDto.StudentUniversityId,
                QuizCode = attemptDto.QuizCode
            };

            _dbContext.StudentQuizAttempts.Add(attempt);
            await _dbContext.SaveChangesAsync();
            return attempt;
        }

        public async Task<StudentQuizAttempt> UpdateAttemptAsync(StudentQuizAttempt attempt)
        {
            var existingAttempt = await _dbContext.StudentQuizAttempts.FindAsync(attempt.Id);
            if (existingAttempt == null)
            {
                return null;
            }

            // Don't allow attempt code, quiz code, or student ID to be changed
            attempt.AttemptCode = existingAttempt.AttemptCode;
            attempt.QuizCode = existingAttempt.QuizCode;
            attempt.StudentUniversityId = existingAttempt.StudentUniversityId;

            _dbContext.Entry(existingAttempt).CurrentValues.SetValues(attempt);
            await _dbContext.SaveChangesAsync();
            return existingAttempt;
        }

        public async Task<IEnumerable<StudentQuizAttempt>> GetRecentAttemptsByProfessorIdAsync(string professorId)
        {
            // Find all courses for this professor
            var courses = await _dbContext.Courses
                .Where(c => c.ProfessorUniversityId == professorId)
                .ToListAsync();

            if (!courses.Any())
            {
                return new List<StudentQuizAttempt>();
            }

            // Get all course codes
            var courseCodes = courses.Select(c => c.CourseCode).ToList();

            // Find all quizzes in these courses
            var quizzes = await _dbContext.Quizzes
                .Where(q => courseCodes.Contains(q.CourseCode))
                .ToListAsync();

            if (!quizzes.Any())
            {
                return new List<StudentQuizAttempt>();
            }

            // Get all quiz codes
            var quizCodes = quizzes.Select(q => q.QuizCode).ToList();

            // Find all attempts for these quizzes
            var attempts = await _dbContext.StudentQuizAttempts
                .Where(a => quizCodes.Contains(a.QuizCode))
                // Order by most recent first
                .OrderByDescending(a => a.EndTime)
                // Limit to the most recent 20 attempts
                .Take(20)
                .ToListAsync();

            return attempts;
        }

        public async Task<int> CalculateAndUpdateScoreAsync(string attemptCode)
        {
            var attempt = await _dbContext.StudentQuizAttempts
                .FirstOrDefaultAsync(a => a.AttemptCode == attemptCode);

            if (attempt == null)
            {
                throw new ArgumentException($"Attempt with code {attemptCode} not found");
            }

            // Get the quiz to determine its type
            var quiz = await _dbContext.Quizzes
                .FirstOrDefaultAsync(q => q.QuizCode == attempt.QuizCode);

            if (quiz == null)
            {
                throw new ArgumentException($"Quiz not found for attempt {attemptCode}");
            }

            // Get all questions for this quiz
            var questions = await _dbContext.QuizQuestions
                .Where(q => q.QuizCode == attempt.QuizCode)
                .ToListAsync();

            // Get student answers
            var studentAnswers = await _studentAnswerRepository.GetAnswersByAttemptCodeAsync(attemptCode);

            // Calculate total possible points
            int totalPossible = questions.Sum(q => q.Points);
            int totalEarned = studentAnswers.Sum(a => a.PointsEarned);

            // Calculate the percentage score (0-100)
            int score = totalPossible > 0 ? (int)Math.Round((double)totalEarned / totalPossible * 100) : 0;

            // Update the attempt with the score
            attempt.Score = score;
            _dbContext.StudentQuizAttempts.Update(attempt);
            await _dbContext.SaveChangesAsync();

            return score;
        }
    }
}