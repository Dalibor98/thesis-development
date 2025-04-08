using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MTS.Services.CurriculumAPI.Data;
using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO;
using MTS.Services.CurriculumAPI.Repository.IRepository;
using MTS.Services.CurriculumAPI.Utilities;

namespace MTS.Services.CurriculumAPI.Repository
{
    public class WeekRepository : IWeekRepository
    {
        private readonly CurriculumDbContext _dbContext;

        public WeekRepository(CurriculumDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<Week>> GetAllWeeksAsync()
        {
            return await _dbContext.Weeks.ToListAsync();
        }

        public async Task<Week?> GetWeekByIdAsync(int id)
        {
            return await _dbContext.Weeks.FindAsync(id);
        }

        public async Task<Week?> GetWeekByCodeAsync(string weekCode)
        {
            return await _dbContext.Weeks
                .FirstOrDefaultAsync(w => w.WeekCode == weekCode);
        }

        public async Task<IEnumerable<Week>?> GetWeeksByCourseCodeAsync(string courseCode)
        {
     
            bool courseExists = await _dbContext.Courses.AnyAsync(c => c.CourseCode == courseCode);

            if (!courseExists)
            {   
                return null;
            }

            return await _dbContext.Weeks
                .Where(w => w.CourseCode == courseCode)
                .OrderBy(w => w.WeekNumber)
                .ToListAsync();
        }

        public async Task<Week>  CreateWeekAsync(WeekCreateDto weekDto)
        {
            if (weekDto.CourseCode.IsNullOrEmpty())
            {
                throw new ArgumentException("Course code not provided.");

            }
           
            var latestWeek = await _dbContext.Weeks
                .Where(w => w.CourseCode == weekDto.CourseCode)
                .OrderByDescending(w => w.WeekNumber)
                .FirstOrDefaultAsync();

            int weekNumber = 1;
            if (latestWeek != null)
            {
                weekNumber = latestWeek.WeekNumber + 1;
            }

            var weekCode = await CodeGenerator.GenerateUniqueWeekCode(_dbContext, weekDto.CourseCode, weekNumber);
          
            
            Week week = new Week
            {
                WeekCode = weekCode,
                WeekNumber = weekNumber,
                CourseCode = weekDto.CourseCode
            };

            _dbContext.Weeks.Add(week);
            await _dbContext.SaveChangesAsync();
            return week;
        }

        public async Task<Week> UpdateWeekAsync(WeekUpdateDto weekDto)
        {
            //What should I update here? Should I allow week properties to be modified at all ?
            //When creating the week, week number is automatically assigned
            //weekCode is required for week to be modified and should not be modifieed 
            //Week belongs to a certain course and thus course code should not change?
            //If I change the number than I have to change numbers of all the others accordingly
            //(If weeks 1-6 already exists and if I want to change 5th week to 1st than 1st becomes 2nd and so on but this is all additional headache..This should not be allowed I think.
            var existingWeek = await _dbContext.Weeks.FirstOrDefaultAsync(w => w.WeekCode == weekDto.WeekCode);
            if (existingWeek == null)
            {
                return null;
            }

            // Don't allow course code or week code to be changed
            weekDto.CourseCode = existingWeek.CourseCode;
            weekDto.WeekCode = existingWeek.WeekCode;

            _dbContext.Entry(existingWeek).CurrentValues.SetValues(weekDto);
            await _dbContext.SaveChangesAsync();
            return existingWeek;
        }

        public async Task<bool> DeleteWeekAsync(int id)
        {
            var week = await _dbContext.Weeks.FindAsync(id);
            if (week == null)
            {
                return false;
            }

            // Get all related entities by week code
            var materials = await _dbContext.Materials
                .Where(m => m.WeekCode == week.WeekCode)
                .ToListAsync();

            var assignments = await _dbContext.Assignments
                .Where(a => a.WeekCode == week.WeekCode)
                .ToListAsync();

            var quizzes = await _dbContext.Quizzes
                .Where(q => q.WeekCode == week.WeekCode)
                .ToListAsync();

            // Get quiz codes for this week
            var quizCodes = quizzes.Select(q => q.QuizCode).ToList();

            // Get quiz questions by quiz codes
            var quizQuestions = new List<QuizQuestion>();
            if (quizCodes.Any())
            {
                quizQuestions = await _dbContext.QuizQuestions
                    .Where(qq => quizCodes.Contains(qq.QuizCode))
                    .ToListAsync();
            }

            // Get question codes
            var questionCodes = quizQuestions.Select(qq => qq.QuizQuestionCode).ToList();

            // Get answers by question codes
            var answers = new List<Answer>();
            if (questionCodes.Any())
            {
                answers = await _dbContext.Answers
                    .Where(a => questionCodes.Contains(a.QuizQuestionCode))
                    .ToListAsync();
            }

            // Get assignment codes for this week
            var assignmentCodes = assignments.Select(a => a.AssignmentCode).ToList();

            // Get student assignments by assignment codes
            var studentAssignments = new List<StudentAssignmentAttempt>();
            if (assignmentCodes.Any())
            {
                studentAssignments = await _dbContext.StudentAssignmentAttempts
                    .Where(sa => assignmentCodes.Contains(sa.AssignmentCode))
                    .ToListAsync();
            }

            // Remove all related entities
            _dbContext.StudentAssignmentAttempts.RemoveRange(studentAssignments);
            _dbContext.Answers.RemoveRange(answers);
            _dbContext.QuizQuestions.RemoveRange(quizQuestions);
            _dbContext.Quizzes.RemoveRange(quizzes);
            _dbContext.Assignments.RemoveRange(assignments);
            _dbContext.Materials.RemoveRange(materials);
            _dbContext.Weeks.Remove(week);

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Material>> GetMaterialsByWeekCodeAsync(string weekCode)
        {
            return await _dbContext.Materials
                .Where(m => m.WeekCode == weekCode)
                .ToListAsync();
        }

        public async Task<IEnumerable<Assignment>> GetAssignmentsByWeekCodeAsync(string weekCode)
        {
            return await _dbContext.Assignments
                .Where(a => a.WeekCode == weekCode)
                .ToListAsync();
        }

        public async Task<IEnumerable<Quiz>> GetQuizzesByWeekCodeAsync(string weekCode)
        {
            return await _dbContext.Quizzes
                .Where(q => q.WeekCode == weekCode)
                .ToListAsync();
        }
    }
}
