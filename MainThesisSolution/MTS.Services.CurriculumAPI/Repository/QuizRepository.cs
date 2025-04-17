using MTS.Services.CurriculumAPI.Data;
using MTS.Services.CurriculumAPI.Models.DTO.QuizDto;
using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Repository.IRepository;
using MTS.Services.CurriculumAPI.Utilities;
using Microsoft.EntityFrameworkCore;

public class QuizRepository : IQuizRepository
{
    //CURRENT
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
        var quiz = await _dbContext.Quizzes.FindAsync(id);
        if (quiz == null)
        {
            return false;
        }

        // Get all questions for this quiz
        var questions = await _dbContext.QuizQuestions
            .Where(q => q.QuizCode == quiz.QuizCode)
            .ToListAsync();

        var questionCodes = questions.Select(q => q.QuizQuestionCode).ToList();

        // Get all answer options for these questions
        var answerOptions = new List<AnswerOption>();
        if (questionCodes.Any())
        {
            answerOptions = await _dbContext.AnswerOptions
                .Where(ao => questionCodes.Contains(ao.QuizQuestionCode))
                .ToListAsync();
        }

        // Get all attempts for this quiz
        var attempts = await _dbContext.StudentQuizAttempts
            .Where(a => a.QuizCode == quiz.QuizCode)
            .ToListAsync();

        var attemptCodes = attempts.Select(a => a.AttemptCode).ToList();

        // Get all student answers for these attempts
        var studentAnswers = new List<StudentAnswer>();
        if (attemptCodes.Any())
        {
            studentAnswers = await _dbContext.StudentAnswers
                .Where(sa => attemptCodes.Contains(sa.AttemptCode))
                .ToListAsync();
        }

        // Remove all related entities in the correct order (child to parent)
        _dbContext.StudentAnswers.RemoveRange(studentAnswers);
        _dbContext.StudentQuizAttempts.RemoveRange(attempts);
        _dbContext.AnswerOptions.RemoveRange(answerOptions);
        _dbContext.QuizQuestions.RemoveRange(questions);
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
    // Add to QuizRepository.cs
    public async Task<IEnumerable<QuizWithAttemptsViewModel>> GetTextBasedQuizzesWithPendingGradingAsync(string professorId)
    {
        // Get all courses for this professor
        var courses = await _dbContext.Courses
            .Where(c => c.ProfessorUniversityId == professorId)
            .ToListAsync();

        if (!courses.Any())
        {
            return new List<QuizWithAttemptsViewModel>();
        }

        // Get all course codes
        var courseCodes = courses.Select(c => c.CourseCode).ToList();

        // Get all text-based quizzes for these courses
        var quizzes = await _dbContext.Quizzes
            .Where(q => courseCodes.Contains(q.CourseCode) && q.QuizType == "TextBased")
            .ToListAsync();

        if (!quizzes.Any())
        {
            return new List<QuizWithAttemptsViewModel>();
        }

        // Prepare the result list
        var result = new List<QuizWithAttemptsViewModel>();

        foreach (var quiz in quizzes)
        {
            // Get all attempts for this quiz
            var attempts = await _dbContext.StudentQuizAttempts
                .Where(a => a.QuizCode == quiz.QuizCode)
                .ToListAsync();

            if (!attempts.Any())
            {
                continue;
            }

            // Get attempts that have ungraded text answers
            var attemptCodes = attempts.Select(a => a.AttemptCode).ToList();
            var pendingAnswers = await _dbContext.StudentAnswers
                .Where(a => attemptCodes.Contains(a.AttemptCode)
                      && !string.IsNullOrEmpty(a.TextAnswer)
                      && a.GradingStatus == "Ungraded")
                .ToListAsync();

            if (!pendingAnswers.Any())
            {
                continue;
            }

            // Get the distinct attempts that have pending answers
            var pendingAttemptCodes = pendingAnswers.Select(a => a.AttemptCode).Distinct().ToList();
            var pendingAttempts = attempts
                .Where(a => pendingAttemptCodes.Contains(a.AttemptCode))
                .ToList();

            result.Add(new QuizWithAttemptsViewModel
            {
                Quiz = new QuizDto
                {
                    QuizCode = quiz.QuizCode,
                    Title = quiz.Title,
                    StartTime = quiz.StartTime,
                    EndTime = quiz.EndTime,
                    TimeLimit = quiz.TimeLimit,
                    QuizType = quiz.QuizType,
                    CourseCode = quiz.CourseCode,
                    WeekCode = quiz.WeekCode

                },
                PendingAttempts = pendingAttempts.Select(a => new StudentQuizAttemptDto
                {
                    Id = a.Id,
                    AttemptCode = a.AttemptCode,
                    StudentUniversityId = a.StudentUniversityId,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Score = a.Score
                }).ToList()
            });
        }

        return result;
    }
}