using Microsoft.EntityFrameworkCore;
using MTS.Services.CurriculumAPI.Data;
using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Repository.IRepository;

namespace MTS.Services.CurriculumAPI.Repository
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly CurriculumDbContext _dbContext;

        public AssignmentRepository(CurriculumDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<Assignment>> GetAllAssignmentsAsync()
        {
            return await _dbContext.Assignments.ToListAsync();
        }

        public async Task<Assignment?> GetAssignmentByIdAsync(int id)
        {
            return await _dbContext.Assignments.FindAsync(id);
        }

        public async Task<Assignment?> GetAssignmentByCodeAsync(string assignmentCode)
        {
            return await _dbContext.Assignments
                .FirstOrDefaultAsync(a => a.AssignmentCode == assignmentCode);
        }

        public async Task<IEnumerable<Assignment>> GetAssignmentsByCourseCodeAsync(string courseCode)
        {
            return await _dbContext.Assignments
                .Where(a => a.CourseCode == courseCode)
                .ToListAsync();
        }

        public async Task<IEnumerable<Assignment>> GetAssignmentsByWeekCodeAsync(string weekCode)
        {
            return await _dbContext.Assignments
                .Where(a => a.WeekCode == weekCode)
                .ToListAsync();
        }

        public async Task<Assignment> CreateAssignmentAsync(Assignment assignment)
        {
            // Generate assignment code if not provided
            if (string.IsNullOrEmpty(assignment.AssignmentCode))
            {
                assignment.AssignmentCode = Assignment.GenerateAssignmentCode(assignment.WeekCode);
            }

            // Ensure course code is set if we have a week code
            if (!string.IsNullOrEmpty(assignment.WeekCode) && string.IsNullOrEmpty(assignment.CourseCode))
            {
                var week = await _dbContext.Weeks
                    .FirstOrDefaultAsync(w => w.WeekCode == assignment.WeekCode);

                if (week != null)
                {
                    assignment.CourseCode = week.CourseCode;
                }
            }

            _dbContext.Assignments.Add(assignment);
            await _dbContext.SaveChangesAsync();
            return assignment;
        }

        public async Task<Assignment> UpdateAssignmentAsync(Assignment assignment)
        {
            var existingAssignment = await _dbContext.Assignments.FindAsync(assignment.Id);
            if (existingAssignment == null)
            {
                return null;
            }

            // Don't allow course code, week code, or assignment code to be changed
            assignment.CourseCode = existingAssignment.CourseCode;
            assignment.WeekCode = existingAssignment.WeekCode;
            assignment.AssignmentCode = existingAssignment.AssignmentCode;

            _dbContext.Entry(existingAssignment).CurrentValues.SetValues(assignment);
            await _dbContext.SaveChangesAsync();
            return existingAssignment;
        }

        public async Task<bool> DeleteAssignmentAsync(int id)
        {
            var assignment = await _dbContext.Assignments.FindAsync(id);
            if (assignment == null)
            {
                return false;
            }

            // Get student submissions
            var studentSubmissions = await _dbContext.StudentAssignmentAttempts
                .Where(sa => sa.AssignmentCode == assignment.AssignmentCode)
                .ToListAsync();

            // Remove all related entities
            _dbContext.StudentAssignmentAttempts.RemoveRange(studentSubmissions);
            _dbContext.Assignments.Remove(assignment);

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<StudentAssignmentAttempt>> GetSubmissionsByAssignmentCodeAsync(string assignmentCode)
        {
            return await _dbContext.StudentAssignmentAttempts
                .Where(sa => sa.AssignmentCode == assignmentCode)
                .ToListAsync();
        }

        public async Task<StudentAssignmentAttempt?> GetStudentSubmissionAsync(string assignmentCode, string studentUniversityId)
        {
            return await _dbContext.StudentAssignmentAttempts
                .FirstOrDefaultAsync(sa =>
                    sa.AssignmentCode == assignmentCode &&
                    sa.StudentUniversityId == studentUniversityId);
        }

        
    }
}
