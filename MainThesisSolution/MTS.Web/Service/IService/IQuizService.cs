using MTS.Web.Models;
using MTS.Web.Models.Curriculum.Quiz.Used;

namespace MTS.Web.Service.IService
{
    public interface IQuizService
    {
        Task<ResponseDto?> GetAllQuizzesAsync();
        Task<ResponseDto?> GetQuizByIdAsync(int id);
        Task<ResponseDto?> GetQuizByCodeAsync(string quizCode);
        Task<ResponseDto?> GetQuizzesByWeekCodeAsync(string weekCode);
        Task<ResponseDto?> GetQuizzesByCourseCodeAsync(string courseCode);
        Task<ResponseDto?> CreateQuizAsync(QuizCreateDto quizDto);
        Task<ResponseDto?> UpdateQuizAsync(QuizUpdateDto quizDto);
        Task<ResponseDto?> DeleteQuizAsync(string quizCode);
        Task<ResponseDto?> GetUpcomingQuizzesByStudentIdAsync(string studentId);
        Task<ResponseDto?> GetRecentQuizAttemptsByProfessorIdAsync(string professorId);
        Task<ResponseDto?> GetTextBasedQuizzesWithPendingGradingAsync(string professorId);
    }
}