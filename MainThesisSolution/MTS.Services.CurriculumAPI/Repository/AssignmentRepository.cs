using Microsoft.EntityFrameworkCore;
using MTS.Services.CurriculumAPI.Data;
using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO;
using MTS.Services.CurriculumAPI.Repository.IRepository;
using MTS.Services.CurriculumAPI.Utilities;

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

        public async Task<Assignment> CreateAssignmentAsync(AssignmentCreateDto assignmentDto)
        {
            //ALL this is under one big question mark ??? See how I am going to feed the data from UI, codes should be auto-inserted or added manually? 

            var week = await _dbContext.Weeks.FirstOrDefaultAsync(w => w.WeekCode == assignmentDto.WeekCode);

            if (week == null)
            {
                throw new ArgumentNullException("Week with the given weekCode doesn't exist");
            }
            if (string.IsNullOrEmpty(assignmentDto.CourseCode))
            {
                assignmentDto.CourseCode = week.CourseCode;
            }
           
            var assignmentCode = await CodeGenerator.GenerateUniqueAssignmentCode(_dbContext,week.WeekCode);

            Assignment assignment = new Assignment
            {
                AssignmentCode = assignmentCode,
                CourseCode = assignmentDto.CourseCode,
                WeekCode = assignmentDto.WeekCode,
                Title = assignmentDto.Title,
                Description = assignmentDto.Description,
                MaxPoints = assignmentDto.MaxPoints,
                MinPoints = assignmentDto.MinPoints,
                DueDate = assignmentDto.DueDate
            };

            _dbContext.Assignments.Add(assignment);
            await _dbContext.SaveChangesAsync();
            return assignment;
        }

        public async Task<Assignment> UpdateAssignmentAsync(AssignmentUpdateDto assignment)
        {
            var existingAssignment = await _dbContext.Assignments.FirstOrDefaultAsync(a => a.AssignmentCode == assignment.AssignmentCode);
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
        public async Task<bool> DeleteAssignmentByCodeAsync(string assignmentCode)
        {
            var assignment = await _dbContext.Assignments.FirstOrDefaultAsync(a => a.AssignmentCode == assignmentCode);
            if (assignment == null)
            {
                return false;
            }
            var studentSubmissions = new List<StudentAssignmentAttempt>();
            if (!string.IsNullOrEmpty(assignmentCode))
            {
                studentSubmissions = await _dbContext.StudentAssignmentAttempts
                    .Where(sa => sa.AssignmentCode == assignmentCode)
                    .ToListAsync();
            }

            // Remove all related entities
            if (studentSubmissions.Any())
                {
                    _dbContext.StudentAssignmentAttempts.RemoveRange(studentSubmissions);
                }

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
