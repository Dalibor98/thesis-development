using MTS.Web.Models;
using MTS.Web.Models.Curriculum.Quiz;

namespace MTS.Web.Service.IService
{
    public interface IQuizService
    {
        Task<ResponseDto?> GetQuizByCodeAsync(string quizCode);
        Task<ResponseDto?> GetQuizzesByWeekCodeAsync(string weekCode);
        Task<ResponseDto?> GetQuizzesByCourseCodeAsync(string courseCode);
        Task<ResponseDto?> CreateQuizAsync(QuizCreateDto quizDto);
        Task<ResponseDto?> UpdateQuizAsync(QuizUpdateDto quizDto);
        Task<ResponseDto?> DeleteQuizAsync(string quizCode);

        // Question management
        Task<ResponseDto?> GetQuestionsByQuizCodeAsync(string quizCode);
        Task<ResponseDto?> GetQuestionByCodeAsync(string questionCode);
        Task<ResponseDto?> CreateQuestionAsync(QuizQuestionCreateDto questionDto);

        // Student attempts
        Task<ResponseDto?> GetAttemptsByQuizCodeAsync(string quizCode);
        Task<ResponseDto?> GetAttemptsByStudentIdAsync(string studentUniversityId);
        Task<ResponseDto?> CreateAttemptAsync(StudentQuizAttemptCreateDto attemptDto);
    }
}
