using MTS.Web.Models;
using MTS.Web.Models.Curriculum.Quiz.Used;

public interface IQuizService
{
    Task<ResponseDto?> GetAllQuizzesAsync();
    Task<ResponseDto?> GetQuizByIdAsync(int id);
    Task<ResponseDto?> GetQuizByCodeAsync(string quizCode);
    Task<ResponseDto?> GetQuizzesByWeekCodeAsync(string weekCode);
    Task<ResponseDto?> GetQuizzesByCourseCodeAsync(string courseCode);
    Task<ResponseDto?> CreateQuizAsync(QuizCreateDto quizDto);
    Task<ResponseDto?> UpdateQuizAsync(QuizUpdateDto quizDto);
    Task<ResponseDto?> DeleteQuizAsync(int id);
    Task<ResponseDto?> GetUpcomingQuizzesByStudentIdAsync(string studentId);
}