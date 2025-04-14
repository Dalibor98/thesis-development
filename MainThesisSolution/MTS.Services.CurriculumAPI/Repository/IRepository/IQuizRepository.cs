using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO;
using MTS.Services.CurriculumAPI.Models.DTO.QuizDto;

namespace MTS.Services.CurriculumAPI.Repository.IRepository
{
    public interface IQuizRepository
    {
        Task<IEnumerable<Quiz>> GetAllQuizzesAsync();
        Task<Quiz?> GetQuizByIdAsync(int id);
        Task<Quiz?> GetQuizByCodeAsync(string quizCode);
        Task<IEnumerable<Quiz>> GetQuizzesByCourseCodeAsync(string courseCode);
        Task<IEnumerable<Quiz>> GetQuizzesByWeekCodeAsync(string weekCode);
        Task<Quiz> CreateQuizAsync(QuizCreateDto quizDto);
        Task<Quiz> UpdateQuizAsync(QuizUpdateDto quizDto);
        Task<bool> DeleteQuizAsync(int id);

        // Related data
        Task<IEnumerable<QuizQuestion>> GetQuestionsByQuizCodeAsync(string quizCode);
        Task<QuizQuestion?> GetQuestionByCodeAsync(string questionCode);
        Task<QuizQuestion> CreateQuestionAsync(QuizQuestionCreateDto question);
        Task<QuizQuestion> UpdateQuestionAsync(QuizQuestionUpdateDto question);
        Task<bool> DeleteQuestionByCodeAsync(string questionCode);

        Task<IEnumerable<Answer>> GetAnswersByQuestionCodeAsync(string questionCode);
        Task<Answer> CreateAnswerAsync(Answer answer);
        Task<Answer> UpdateAnswerAsync(Answer answer);
        Task<bool> DeleteAnswerAsync(int id);

        Task<IEnumerable<StudentQuizAttempt>> GetAttemptsByQuizCodeAsync(string quizCode);
        Task<IEnumerable<StudentQuizAttempt>> GetAttemptsByStudentIdAsync(string studentUniversityId);
        Task<StudentQuizAttempt?> GetAttemptByCodeAsync(string attemptCode);
        Task<StudentQuizAttempt> CreateAttemptAsync(StudentQuizAttemptCreateDto attempt);
        Task<StudentQuizAttempt> UpdateAttemptAsync(StudentQuizAttempt attempt);

        Task<IEnumerable<StudentQuizAnswer>> GetAnswersByAttemptCodeAsync(string attemptCode);
        Task<StudentQuizAnswer> CreateStudentAnswerAsync(StudentQuizAnswerCreateDto answer);
        Task<StudentQuizAnswer> UpdateStudentAnswerAsync(StudentQuizAnswer answer);
        Task<IEnumerable<StudentQuizAttempt>> GetRecentAttemptsByProfessorIdAsync(string professorId);
        Task<IEnumerable<Quiz>> GetUpcomingQuizzesByStudentIdAsync(string studentId);
        Task<StudentQuizAnswer> GradeStudentAnswerAsync(StudentQuizAnswerGradeDto gradeDto);
        Task<StudentQuizAnswer?> GetStudentAnswerByIdAsync(int id);



    }
}