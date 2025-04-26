using Microsoft.EntityFrameworkCore;
using MTS.Services.CurriculumAPI.Data;
using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO.CourseDto;
using MTS.Services.CurriculumAPI.Repository.IRepository;
using MTS.Services.CurriculumAPI.Services.IService;
using MTS.Services.CurriculumAPI.Utilities;

namespace MTS.Services.CurriculumAPI.Repository
{
    public class CourseRepository : ICourseRepository
    {//CURRENT
        private readonly CurriculumDbContext _dbContext;
        private readonly IUserAPIService _userAPIService;

        public CourseRepository(CurriculumDbContext dbContext, IUserAPIService userAPIService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _userAPIService = userAPIService ?? throw new ArgumentNullException(nameof(userAPIService));

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
            var course = await _dbContext.Courses.FirstOrDefaultAsync(c => c.CourseCode == courseCode);
            return course;
        }
        public async Task<IEnumerable<Course>> GetCoursesByProfessorIdAsync(string professorUniversityId)
        {
            return await _dbContext.Courses
                .Where(c => c.ProfessorUniversityId == professorUniversityId)
                .ToListAsync();
        }

        public async Task<Course> CreateCourseAsync(CourseCreateDto courseCreateDto)
        {
            if (string.IsNullOrEmpty(courseCreateDto.ProfessorUniversityId))
            {
                throw new ArgumentException("Professor ID is required");
            }

            var professorExists = await _userAPIService.ProfessorExistsAsync(courseCreateDto.ProfessorUniversityId);
            if (!professorExists)
            {
                throw new ArgumentException($"Professor with ID {courseCreateDto.ProfessorUniversityId} does not exist");
            }

            // Check if a course with the same title already exists
            bool courseExists = await _dbContext.Courses.AnyAsync(c => c.Title.ToLower() == courseCreateDto.Title.ToLower());
            if (courseExists)
            {
                throw new InvalidOperationException("A course with this title already exists. Please choose a different title.");
            }

            var code = await CodeGenerator.GenerateUniqueCourseCode(_dbContext);
            Course course = new Course
            {
                CourseCode = code,
                Description = courseCreateDto.Description,
                Title = courseCreateDto.Title,
                ProfessorUniversityId = courseCreateDto.ProfessorUniversityId
            };

            _dbContext.Courses.Add(course);
            await _dbContext.SaveChangesAsync();
            return course;
        }

        public async Task<Course> UpdateCourseAsync(CourseUpdateDto course)
        {
            var existingCourse = await _dbContext.Courses.FirstOrDefaultAsync(c=> c.CourseCode == course.CourseCode);
            if (existingCourse == null)
            {
                return null;
            }
            
            // A step to guard changing the course code and professorId
            course.CourseCode = existingCourse.CourseCode;

            course.ProfessorUniversityId = existingCourse.ProfessorUniversityId;

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

            var quizzes = await _dbContext.Quizzes
                .Where(q => q.CourseCode == course.CourseCode)
                .ToListAsync();

            var registrations = await _dbContext.CourseRegistrations
                .Where(r => r.CourseCode == course.CourseCode)
                .ToListAsync();

            // Get quiz questions by quiz codes
            var quizQuestions = new List<QuizQuestion>();

            var quizCodes = quizzes.Select(q => q.QuizCode).ToList();

            if (quizCodes.Any())
            {
                quizQuestions = await _dbContext.QuizQuestions
                    .Where(qq => quizCodes.Contains(qq.QuizCode))
                    .ToListAsync();
            }

            


            _dbContext.QuizQuestions.RemoveRange(quizQuestions);
            _dbContext.CourseRegistrations.RemoveRange(registrations);
            _dbContext.Quizzes.RemoveRange(quizzes);
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
