using Microsoft.EntityFrameworkCore;
using MTS.Services.CurriculumAPI.Data;
using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO.AssignmentDto;
using MTS.Services.CurriculumAPI.Repository.IRepository;
using MTS.Services.CurriculumAPI.Utilities;

namespace MTS.Services.CurriculumAPI.Repository
{
    public class AssignmentRepository : IAssignmentRepository
    {//CURRENT
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

        public async Task<IEnumerable<Assignment>> GetUpcomingAssignmentsByStudentIdAsync(string studentId)
        {
            // Get all course registrations for this student
            var registrations = await _dbContext.CourseRegistrations
                .Where(r => r.StudentCode == studentId && r.RegistrationStatus == "Active")
                .ToListAsync();

            if (!registrations.Any())
            {
                return new List<Assignment>();
            }

            // Get course codes for active enrollments
            var courseCodes = registrations.Select(r => r.CourseCode).ToList();

            // Get all assignments for these courses
            var assignments = await _dbContext.Assignments
                .Where(a => courseCodes.Contains(a.CourseCode))
                .ToListAsync();

            // Get all submissions by this student
            var submissions = await _dbContext.StudentAssignmentAttempts
                .Where(s => s.StudentUniversityId == studentId)
                .ToListAsync();

            // Get assignment codes that the student has already submitted
            var submittedAssignmentCodes = submissions.Select(s => s.AssignmentCode).ToList();

            // Filter to assignments that are not submitted yet and due in the future or recently past
            var now = DateTime.Now;
            var cutoffDate = now.AddDays(-7); // Show assignments up to 7 days after due date
            var upcomingAssignments = assignments
                .Where(a => !submittedAssignmentCodes.Contains(a.AssignmentCode) && a.DueDate > cutoffDate)
                .OrderBy(a => a.DueDate)
                .ToList();

            return upcomingAssignments;
        }

        public async Task<IEnumerable<StudentAssignmentAttempt>> GetRecentSubmissionsByProfessorIdAsync(string professorId)
        {
            // Find all courses taught by this professor
            var courses = await _dbContext.Courses
                .Where(c => c.ProfessorUniversityId == professorId)
                .ToListAsync();

            if (!courses.Any())
            {
                return new List<StudentAssignmentAttempt>();
            }

            // Get course codes
            var courseCodes = courses.Select(c => c.CourseCode).ToList();

            // Find all assignments in these courses
            var assignments = await _dbContext.Assignments
                .Where(a => courseCodes.Contains(a.CourseCode))
                .ToListAsync();

            if (!assignments.Any())
            {
                return new List<StudentAssignmentAttempt>();
            }

            // Get assignment codes
            var assignmentCodes = assignments.Select(a => a.AssignmentCode).ToList();

            // Find all submissions for these assignments
            var submissions = await _dbContext.StudentAssignmentAttempts
                .Where(s => assignmentCodes.Contains(s.AssignmentCode))
                .OrderByDescending(s => s.SubmissionDate)
                .Take(20) // Limit to 20 most recent submissions
                .ToListAsync();

            return submissions;
        }
    }
}
