using MTS.Services.UserAPI.Data;
using MTS.Services.UserAPI.Models.DTO;
using MTS.Services.UserAPI.Models;
using MTS.Services.UserAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MTS.Services.UserAPI.Repository
{
    public class StudentRepository : IStudentRepository
    {
        private readonly UserDbContext _db;
        private readonly IUniversityIdRepository _universityIdRepository;

        public StudentRepository(UserDbContext db, IUniversityIdRepository universityIdRepository)
        {
            _db = db;
            _universityIdRepository = universityIdRepository;
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _db.Students.ToListAsync();
        }

        public async Task<Student> GetStudentByIdAsync(string id)
        {
            return await _db.Students.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Student> GetStudentByUniversityIdAsync(string universityId)
        {
            return await _db.Students.FirstOrDefaultAsync(s => s.UniversityId == universityId);
        }

        public async Task<Student> CreateStudentAsync(StudentCreateDto studentDto)
        {
            // First verify that the universityId is valid and of type STUDENT
            bool isValid = await _universityIdRepository.VerifyUniversityIdAsync(studentDto.UniversityId, "STUDENT");

            if (!isValid)
            {
                return null; // Invalid university ID
            }

            // Create new student
            Student student = new Student
            {
                //Id = ??
                Name = studentDto.Name,
                UniversityId = studentDto.UniversityId,
                Major = studentDto.Major,
                EnrollmentYear = studentDto.EnrollmentYear,
            };

            _db.Students.Add(student);
            await _db.SaveChangesAsync();

            // Mark the university ID as assigned
            await _universityIdRepository.AssignUniversityIdAsync(studentDto.UniversityId);

            return student;
        }

        public async Task<bool> UpdateStudentAsync(string id, StudentCreateDto studentDto)
        {
            var student = await _db.Students.FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                return false;
            }

            // Update student properties except for universityId (which shouldn't change)
            //Do this more cleanly
            student.Name = studentDto.Name;
            student.Major = studentDto.Major;
            student.EnrollmentYear = studentDto.EnrollmentYear;

            _db.Students.Update(student);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteStudentAsync(string id)
        {
            var student = await _db.Students.FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                return false;
            }

            _db.Students.Remove(student);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}
