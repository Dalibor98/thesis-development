using MTS.Services.CurriculumAPI.Data;
using MTS.Services.CurriculumAPI.Models.DTO.QuizDto;
using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Repository.IRepository;
using MTS.Services.CurriculumAPI.Utilities;
using Microsoft.EntityFrameworkCore;

public class QuizRepository : IQuizRepository
{
    private readonly CurriculumDbContext _dbContext;

    public QuizRepository(CurriculumDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IEnumerable<Quiz>> GetAllQuizzesAsync()
    {
        return await _dbContext.Quizzes.ToListAsync();
    }

    public async Task<Quiz?> GetQuizByIdAsync(int id)
    {
        return await _dbContext.Quizzes.FindAsync(id);
    }

    public async Task<Quiz?> GetQuizByCodeAsync(string quizCode)
    {
        return await _dbContext.Quizzes
            .FirstOrDefaultAsync(q => q.QuizCode == quizCode);
    }

    public async Task<IEnumerable<Quiz>> GetQuizzesByCourseCodeAsync(string courseCode)
    {
        return await _dbContext.Quizzes
            .Where(q => q.CourseCode == courseCode)
            .ToListAsync();
    }

    public async Task<IEnumerable<Quiz>> GetQuizzesByWeekCodeAsync(string weekCode)
    {
        return await _dbContext.Quizzes
            .Where(q => q.WeekCode == weekCode)
            .ToListAsync();
    }

    public async Task<Quiz> CreateQuizAsync(QuizCreateDto quizDto)
    {
        // Validate that the week exists
        var week = await _dbContext.Weeks.FirstOrDefaultAsync(w => w.WeekCode == quizDto.WeekCode);
        if (week == null)
        {
            throw new ArgumentException("Week with the given weekCode doesn't exist");
        }

        // Set the course code from the week if not provided
        if (string.IsNullOrEmpty(quizDto.CourseCode))
        {
            quizDto.CourseCode = week.CourseCode;
        }

        // Validate quiz type
        if (quizDto.QuizType != "MultipleChoice" && quizDto.QuizType != "TextBased")
        {
            quizDto.QuizType = "MultipleChoice"; // Default to MultipleChoice if invalid
        }

        // Generate a unique quiz code
        string quizCode = await CodeGenerator.GenerateUniqueQuizCode(_dbContext, quizDto.WeekCode);

        // Create the quiz
        Quiz quiz = new Quiz
        {
            QuizCode = quizCode,
            CourseCode = quizDto.CourseCode,
            WeekCode = quizDto.WeekCode,
            Title = quizDto.Title,
            StartTime = quizDto.StartTime,
            EndTime = quizDto.EndTime,
            TimeLimit = quizDto.TimeLimit,
            QuizType = quizDto.QuizType
        };

        _dbContext.Quizzes.Add(quiz);
        await _dbContext.SaveChangesAsync();
        return quiz;
    }

    public async Task<Quiz> UpdateQuizAsync(QuizUpdateDto quizDto)
    {
        var existingQuiz = await _dbContext.Quizzes.FirstOrDefaultAsync(q => q.QuizCode == quizDto.QuizCode);
        if (existingQuiz == null)
        {
            return null;
        }

        // Don't allow quiz code, week code, or course code to be changed
        quizDto.CourseCode = existingQuiz.CourseCode;
        quizDto.WeekCode = existingQuiz.WeekCode;

        // Validate quiz type
        if (quizDto.QuizType != "MultipleChoice" && quizDto.QuizType != "TextBased")
        {
            quizDto.QuizType = existingQuiz.QuizType; // Keep existing type if invalid
        }

        // Update the properties
        existingQuiz.Title = quizDto.Title;
        existingQuiz.StartTime = quizDto.StartTime;
        existingQuiz.EndTime = quizDto.EndTime;
        existingQuiz.TimeLimit = quizDto.TimeLimit;
        existingQuiz.QuizType = quizDto.QuizType;

        _dbContext.Quizzes.Update(existingQuiz);
        await _dbContext.SaveChangesAsync();
        return existingQuiz;
    }

    public async Task<bool> DeleteQuizAsync(int id)
    {
        // This should be implemented to coordinate with other repositories
        // or use a transaction to ensure all related entities are deleted properly
        var quiz = await _dbContext.Quizzes.FindAsync(id);
        if (quiz == null)
        {
            return false;
        }

        _dbContext.Quizzes.Remove(quiz);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Quiz>> GetUpcomingQuizzesByStudentIdAsync(string studentId)
    {
        var enrollments = await _dbContext.CourseRegistrations
            .Where(r => r.StudentCode == studentId && r.RegistrationStatus == "Active")
            .Select(r => r.CourseCode)
            .ToListAsync();

        if (!enrollments.Any())
        {
            return new List<Quiz>();
        }

        var now = DateTime.Now;
        var upcomingQuizzes = await _dbContext.Quizzes
            .Where(q => enrollments.Contains(q.CourseCode) && q.EndTime >= now)
            .OrderBy(q => q.StartTime)
            .ToListAsync();

        return upcomingQuizzes;
    }
}