using Microsoft.EntityFrameworkCore;
using MTS.Services.CurriculumAPI.Data;
using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO;
using MTS.Services.CurriculumAPI.Repository.IRepository;

namespace MTS.Services.CurriculumAPI.Repository
{
    public class StudentAssignmentAttemptRepository : IStudentAssignmentAttemptRepository
    {
        private readonly CurriculumDbContext _dbContext;

        public StudentAssignmentAttemptRepository(CurriculumDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<StudentAssignmentAttempt> CreateAttemptAsync(StudentAssignmentAttemptCreateDto attemptDto)
        {
            var assignment = await _dbContext.Assignments.FirstOrDefaultAsync(a =>
                a.AssignmentCode == attemptDto.AssignmentCode);

            if (assignment == null)
            {
                throw new ArgumentException("Assignment does not exist");
            }

            // Check if the student already has an attempt for this assignment
            var existingAttempt = await _dbContext.StudentAssignmentAttempts
                .FirstOrDefaultAsync(a =>
                    a.AssignmentCode == attemptDto.AssignmentCode &&
                    a.StudentUniversityId == attemptDto.StudentUniversityId);

            if (existingAttempt != null)
            {
                // Could either throw an exception or update the existing attempt
                throw new InvalidOperationException("Student already has an attempt for this assignment");
            }

            // Determine submission status based on due date
            string submissionStatus = DateTime.Now > assignment.DueDate ? "Late" : "Submitted";

            var attempt = new StudentAssignmentAttempt
            {
                AssignmentCode = attemptDto.AssignmentCode,
                StudentUniversityId = attemptDto.StudentUniversityId,
                SubmissionUrl = attemptDto.SubmissionUrl,
                SubmissionDate = DateTime.Now,
                SubmissionStatus = submissionStatus,
                Grade = 0,  // Initial grade is 0
                Score = 0   // Initial score is 0
            };

            _dbContext.StudentAssignmentAttempts.Add(attempt);
            await _dbContext.SaveChangesAsync();
            return attempt;
        }
        public async Task<StudentAssignmentAttempt> UpdateAttemptAsync(StudentAssignmentAttemptUpdateDto attemptDto)
        {
            var attempt = await _dbContext.StudentAssignmentAttempts.FindAsync(attemptDto.Id);
            if (attempt == null)
            {
                return null;
            }

            // Update only the properties that should be updatable
            attempt.Grade = attemptDto.Grade;
            attempt.SubmissionStatus = "Graded";
            attempt.Feedback = attemptDto.Feedback;

            _dbContext.StudentAssignmentAttempts.Update(attempt);
            await _dbContext.SaveChangesAsync();
            return attempt;
        }

        public async Task<IEnumerable<StudentAssignmentAttempt>> GetAttemptsByStudentIdAsync(string studentUniversityId)
        {
            return await _dbContext.StudentAssignmentAttempts
                .Where(a => a.StudentUniversityId == studentUniversityId)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentAssignmentAttempt>> GetAttemptsByAssignmentCodeAsync(string assignmentCode)
        {
            return await _dbContext.StudentAssignmentAttempts
                .Where(a => a.AssignmentCode == assignmentCode)
                .ToListAsync();
        }

        public async Task<StudentAssignmentAttempt> GetAttemptByIdAsync(int id)
        {
            return await _dbContext.StudentAssignmentAttempts.FindAsync(id);
        }

        public async Task<StudentAssignmentAttempt?> GetAttemptByStudentAndAssignmentAsync(
            string studentUniversityId, string assignmentCode)
        {
            return await _dbContext.StudentAssignmentAttempts
                .FirstOrDefaultAsync(a =>
                    a.StudentUniversityId == studentUniversityId &&
                    a.AssignmentCode == assignmentCode);
        }
    }
}