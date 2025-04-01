using Microsoft.EntityFrameworkCore;
using MTS.Services.CurriculumAPI.Data;
using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Repository.IRepository;

namespace MTS.Services.CurriculumAPI.Repository
{
    public class CourseRepository : ICourseRepository
    {
        private readonly CurriculumDbContext _dbContext;

        public CourseRepository(CurriculumDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return await _dbContext.Courses.ToListAsync();
        }
        
        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            return await _dbContext.Courses.FindAsync(id);
        }

        public async Task<Course?> GetCourseByCodeAsync(string courseCode)
        {
            return await _dbContext.Courses.FirstOrDefaultAsync(c => c.CourseCode == courseCode);
        }
        public async Task<IEnumerable<Course>> GetCoursesByProfessorIdAsync(string professorUniversityId)
        {
            return await _dbContext.Courses
                .Where(c => c.ProfessorUniversityId == professorUniversityId)
                .ToListAsync();
        }

        public async Task<Course> CreateCourseAsync(Course course)
        {
            // Generate course code if not provided
            if (string.IsNullOrEmpty(course.CourseCode))
            {
                course.CourseCode = Course.GenerateCourseCode();
            }

            _dbContext.Courses.Add(course);
            await _dbContext.SaveChangesAsync();
            return course;
        }
        public async Task<Course> UpdateCourseAsync(Course course)
        {
            var existingCourse = await _dbContext.Courses.FindAsync(course.CourseCode);
            if (existingCourse == null)
            {
                return null;
            }

            // A step to guard changing the course code
            course.CourseCode = existingCourse.CourseCode;

            _dbContext.Entry(existingCourse).CurrentValues.SetValues(course);
            await _dbContext.SaveChangesAsync();
            return existingCourse;
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            var course = await _dbContext.Courses.FindAsync(id);
            if (course == null)
            {
                return false;
            }

            var weeks = await _dbContext.Weeks
                .Where(w => w.CourseCode == course.CourseCode)
                .ToListAsync();

            var materials = await _dbContext.Materials
                .Where(m => m.CourseCode == course.CourseCode)
                .ToListAsync();

            var assignments = await _dbContext.Assignments
                .Where(a => a.CourseCode == course.CourseCode)
                .ToListAsync();

            var quizzes = await _dbContext.Quizzes
                .Where(q => q.CourseCode == course.CourseCode)
                .ToListAsync();

            var registrations = await _dbContext.CourseRegistrations
                .Where(r => r.CourseCode == course.CourseCode)
                .ToListAsync();

            var quizCodes = quizzes.Select(q => q.QuizCode).ToList();

            // Get quiz questions by quiz codes
            var quizQuestions = await _dbContext.QuizQuestions
                .Where(qq => quizCodes.Contains(qq.QuizCode))
                .ToListAsync();

            // Get assignment codes for this course
            var assignmentCodes = assignments.Select(a => a.AssignmentCode).ToList();

            // Get student assignments by assignment codes
            var studentAssignments = await _dbContext.StudentAssignmentAttempts
                .Where(sa => assignmentCodes.Contains(sa.AssignmentCode))
                .ToListAsync();


            _dbContext.StudentAssignmentAttempts.RemoveRange(studentAssignments);
            _dbContext.QuizQuestions.RemoveRange(quizQuestions);
            _dbContext.CourseRegistrations.RemoveRange(registrations);
            _dbContext.Quizzes.RemoveRange(quizzes);
            _dbContext.Assignments.RemoveRange(assignments);
            _dbContext.Materials.RemoveRange(materials);
            _dbContext.Weeks.RemoveRange(weeks);
            _dbContext.Courses.Remove(course);

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Week>> GetWeeksByCourseCodeAsync(string courseCode)
        {
            return await _dbContext.Weeks
                .Where(w => w.CourseCode == courseCode)
                .OrderBy(w => w.WeekNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<Material>> GetMaterialsByCourseCodeAsync(string courseCode)
        {
            return await _dbContext.Materials
                .Where(m => m.CourseCode == courseCode)
                .ToListAsync();
        }

        public async Task<IEnumerable<Assignment>> GetAssignmentsByCourseCodeAsync(string courseCode)
        {
            return await _dbContext.Assignments
                .Where(a => a.CourseCode == courseCode)
                .ToListAsync();
        }

        public async Task<IEnumerable<Quiz>> GetQuizzesByCourseCodeAsync(string courseCode)
        {
            return await _dbContext.Quizzes
                .Where(q => q.CourseCode == courseCode)
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseRegistration>> GetRegistrationsByCourseCodeAsync(string courseCode)
        {
            return await _dbContext.CourseRegistrations
                .Where(r => r.CourseCode == courseCode)
                .ToListAsync();
        }
    }
}
