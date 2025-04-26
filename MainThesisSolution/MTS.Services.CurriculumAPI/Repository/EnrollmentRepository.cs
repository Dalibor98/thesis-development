// EnrollmentRepository.cs
using Microsoft.EntityFrameworkCore;
using MTS.Services.CurriculumAPI.Data;
using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO;
using MTS.Services.CurriculumAPI.Repository.IRepository;

namespace MTS.Services.CurriculumAPI.Repository
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        //CURRENT
        private readonly CurriculumDbContext _dbContext;

        public EnrollmentRepository(CurriculumDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<CourseRegistration>> GetAllEnrollmentsAsync()
        {
            return await _dbContext.CourseRegistrations.ToListAsync();
        }

        public async Task<IEnumerable<CourseRegistration>> GetEnrollmentsByStudentIdAsync(string studentUniversityId)
        {
            return await _dbContext.CourseRegistrations
                .Where(r => r.StudentCode == studentUniversityId)
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseRegistration>> GetEnrollmentsByCourseCodeAsync(string courseCode)
        {
            return await _dbContext.CourseRegistrations
                .Where(r => r.CourseCode == courseCode)
                .ToListAsync();
        }

        public async Task<CourseRegistration> GetEnrollmentAsync(string courseCode, string studentUniversityId)
        {
            return await _dbContext.CourseRegistrations
                .FirstOrDefaultAsync(r => r.CourseCode == courseCode && r.StudentCode == studentUniversityId);
        }

        public async Task<CourseRegistration> CreateEnrollmentAsync(EnrollmentCreateDto enrollmentDto)
        {
            // Check if the enrollment already exists
            var existingEnrollment = await _dbContext.CourseRegistrations
                .FirstOrDefaultAsync(r => r.CourseCode == enrollmentDto.CourseCode &&
                                           r.StudentCode == enrollmentDto.StudentCode);

            if (existingEnrollment != null)
            {
                if (existingEnrollment.RegistrationStatus == "Dropped")
                {
                    existingEnrollment.RegistrationStatus = "Active";
                    _dbContext.CourseRegistrations.Update(existingEnrollment);
                    await _dbContext.SaveChangesAsync();
                }
                return existingEnrollment;
            }

            // Check if the course exists
            var courseExists = await _dbContext.Courses
                .AnyAsync(c => c.CourseCode == enrollmentDto.CourseCode);

            if (!courseExists)
            {
                throw new ArgumentException($"Course with code {enrollmentDto.CourseCode} does not exist");
            }

            // Create new enrollment
            var enrollment = new CourseRegistration
            {
                CourseCode = enrollmentDto.CourseCode,
                StudentCode = enrollmentDto.StudentCode,
                RegistrationStatus = "Active" // Default status
            };

            _dbContext.CourseRegistrations.Add(enrollment);
            await _dbContext.SaveChangesAsync();
            return enrollment;
        }

        public async Task<CourseRegistration> UpdateEnrollmentAsync(EnrollmentUpdateDto enrollmentDto)
        {
            var enrollment = await _dbContext.CourseRegistrations.FindAsync(enrollmentDto.Id);
            if (enrollment == null)
            {
                return null;
            }

            var oldStatus = enrollment.RegistrationStatus;

            // Update status
            enrollment.RegistrationStatus = enrollmentDto.RegistrationStatus;

            // If student is being dropped from course, clean up all related data
            if (oldStatus == "Active" && enrollmentDto.RegistrationStatus == "Dropped")
            {
                await CleanupStudentCourseDataAsync(enrollment.CourseCode, enrollment.StudentCode);
            }

            _dbContext.CourseRegistrations.Update(enrollment);
            await _dbContext.SaveChangesAsync();
            return enrollment;
        }

        public async Task<bool> DeleteEnrollmentAsync(int id)
        {
            var enrollment = await _dbContext.CourseRegistrations.FindAsync(id);
            if (enrollment == null)
            {
                return false;
            }

            _dbContext.CourseRegistrations.Remove(enrollment);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsStudentEnrolledAsync(string courseCode, string studentUniversityId)
        {
            var isEnrolled = await _dbContext.CourseRegistrations
                .AnyAsync(r => r.CourseCode == courseCode &&
                               r.StudentCode == studentUniversityId &&
                               r.RegistrationStatus == "Active");
            return isEnrolled;
        }

        private async Task CleanupStudentCourseDataAsync(string courseCode, string studentUniversityId)
        {
            // 1. Find all quizzes for this course
            var quizzes = await _dbContext.Quizzes
                .Where(q => q.CourseCode == courseCode)
                .ToListAsync();

            var quizCodes = quizzes.Select(q => q.QuizCode).ToList();

            // 2. Find all quiz attempts by this student for these quizzes
            var quizAttempts = new List<StudentQuizAttempt>();
            if (quizCodes.Any())
            {
                quizAttempts = await _dbContext.StudentQuizAttempts
                    .Where(a => quizCodes.Contains(a.QuizCode) && a.StudentUniversityId == studentUniversityId)
                    .ToListAsync();
            }

            // 3. Find all student answers for these attempts
            var attemptCodes = quizAttempts.Select(a => a.AttemptCode).ToList();
            var studentAnswers = new List<StudentAnswer>();
            if (attemptCodes.Any())
            {
                studentAnswers = await _dbContext.StudentAnswers
                    .Where(sa => attemptCodes.Contains(sa.AttemptCode))
                    .ToListAsync();
            }


            // 6. Remove all related data in the correct order (child to parent)
            _dbContext.StudentAnswers.RemoveRange(studentAnswers);
            _dbContext.StudentQuizAttempts.RemoveRange(quizAttempts);

            // Note: We're only removing the student's data, not the enrollment itself
            // The enrollment status will be updated to "Dropped" in the calling method
        }
    }
}