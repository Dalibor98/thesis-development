using MTS.Services.CurriculumAPI.Models;

namespace MTS.Services.CurriculumAPI.Repository.IRepository
{
    public interface IQuizRepository
    {
        Task<IEnumerable<Quiz>> GetAllQuizzesAsync();
        Task<Quiz?> GetQuizByIdAsync(int id);
        Task<Quiz?> GetQuizByCodeAsync(string quizCode);
        Task<IEnumerable<Quiz>> GetQuizzesByCourseCodeAsync(string courseCode);
        Task<IEnumerable<Quiz>> GetQuizzesByWeekCodeAsync(string weekCode);
        Task<Quiz> CreateQuizAsync(Quiz quiz);
        Task<Quiz> UpdateQuizAsync(Quiz quiz);
        Task<bool> DeleteQuizAsync(int id);

        // Related data
        Task<IEnumerable<QuizQuestion>> GetQuestionsByQuizCodeAsync(string quizCode);
        Task<QuizQuestion?> GetQuestionByCodeAsync(string questionCode);
        Task<QuizQuestion> CreateQuestionAsync(QuizQuestion question);
        Task<QuizQuestion> UpdateQuestionAsync(QuizQuestion question);
        Task<bool> DeleteQuestionAsync(int id);

        Task<IEnumerable<Answer>> GetAnswersByQuestionCodeAsync(string questionCode);
        Task<Answer> CreateAnswerAsync(Answer answer);
        Task<Answer> UpdateAnswerAsync(Answer answer);
        Task<bool> DeleteAnswerAsync(int id);

        Task<IEnumerable<StudentQuizAttempt>> GetAttemptsByQuizCodeAsync(string quizCode);
        Task<IEnumerable<StudentQuizAttempt>> GetAttemptsByStudentIdAsync(string studentUniversityId);
        Task<StudentQuizAttempt?> GetAttemptByCodeAsync(string attemptCode);
        Task<StudentQuizAttempt> CreateAttemptAsync(StudentQuizAttempt attempt);
        Task<StudentQuizAttempt> UpdateAttemptAsync(StudentQuizAttempt attempt);

        Task<IEnumerable<StudentQuizAnswer>> GetAnswersByAttemptCodeAsync(string attemptCode);
        Task<StudentQuizAnswer> CreateStudentAnswerAsync(StudentQuizAnswer answer);
        Task<StudentQuizAnswer> UpdateStudentAnswerAsync(StudentQuizAnswer answer);
    }
}
