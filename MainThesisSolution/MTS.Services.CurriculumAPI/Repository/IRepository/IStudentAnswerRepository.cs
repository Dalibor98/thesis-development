using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO.StudentAnswer;
using MTS.Services.CurriculumAPI.Models.DTO.StudentAnswerDto;

namespace MTS.Services.CurriculumAPI.Repository.IRepository
{
    public interface IStudentAnswerRepository
    {
        Task<IEnumerable<StudentAnswer>> GetAnswersByAttemptCodeAsync(string attemptCode);
        Task<IEnumerable<StudentAnswer>> GetAnswersByQuestionCodeAsync(string questionCode);
        Task<IEnumerable<StudentAnswer>> GetAnswersByStudentIdAsync(string studentUniversityId);
        Task<StudentAnswer?> GetAnswerByIdAsync(int id);

        //CHANGE DTO
        Task<StudentAnswer> CreateStudentAnswerAsync(StudentAnswerCreateDto answerDto);

        //HERE TOO
        //Task<StudentAnswer> UpdateStudentAnswerAsync(StudentAnswerUpdateDto answer);

        // Specifically for professor grading
        Task<StudentAnswer> GradeStudentAnswerAsync(StudentAnswerGradeDto gradeDto);

        // Get answers that need grading
        Task<IEnumerable<StudentAnswer>> GetUngradedAnswersAsync(string professorId);

        // Delete a student answer (optional, may not be needed)
        Task<bool> DeleteStudentAnswerAsync(int id);
    }
}