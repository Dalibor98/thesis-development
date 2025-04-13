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
                return existingEnrollment; // Already enrolled
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

            // Only update status
            enrollment.RegistrationStatus = enrollmentDto.RegistrationStatus;

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
    }
}