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
        Task<ResponseDto?> CreateQuestionAsync(QuizQuestionCreateDto questionDto);

        // Student attempts
        Task<ResponseDto?> GetAttemptsByQuizCodeAsync(string quizCode);
        Task<ResponseDto?> GetAttemptsByStudentIdAsync(string studentUniversityId);
        Task<ResponseDto?> CreateAttemptAsync(StudentQuizAttemptCreateDto attemptDto);
        //
        Task<ResponseDto?> GetAttemptByCodeAsync(string attemptCode);
        Task<ResponseDto?> GetAnswersByAttemptCodeAsync(string attemptCode);
        Task<ResponseDto?> SaveStudentAnswerAsync(StudentQuizAnswerCreateDto answerDto);
        Task<ResponseDto?> UpdateAttemptAsync(StudentQuizAttemptDto attemptDto);
        Task<ResponseDto?> CalculateScoreAsync(string attemptCode);
        Task<ResponseDto?> GetQuestionByCodeAsync(string questionCode);
        Task<ResponseDto?> GetAnswersByCodeAsync(string answerCode);
        Task<ResponseDto?> UpdateQuestionAsync(QuizQuestionUpdateDto questionDto);
        Task<ResponseDto?> DeleteQuestionAsync(string questionCode);
        Task<ResponseDto?> GetAnswersForQuestionAsync(string questionCode);
        Task<ResponseDto?> CreateAnswerAsync(AnswerCreateDto answerDto);
        Task<ResponseDto?> UpdateAnswerAsync(AnswerUpdateDto answerDto);
        Task<ResponseDto?> DeleteAnswerAsync(int answerId);
        Task<ResponseDto?> GetUpcomingQuizzesByStudentIdAsync(string studentId);
        Task<ResponseDto?> GetRecentQuizAttemptsByProfessorIdAsync(string professorId);
        Task<ResponseDto?> GradeStudentAnswerAsync(StudentQuizAnswerGradeDto gradeDto);
        Task<ResponseDto?> GetStudentAnswerByIdAsync(int id);
        Task<ResponseDto?> GetTextBasedQuizzesWithPendingGradingAsync(string professorId);

    }

}
