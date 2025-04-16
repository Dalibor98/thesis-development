using Microsoft.EntityFrameworkCore;
using MTS.Services.CurriculumAPI.Data;
using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO.QuizDto;
using MTS.Services.CurriculumAPI.Repository.IRepository;
using MTS.Services.CurriculumAPI.Utilities;

namespace MTS.Services.CurriculumAPI.Repository
{
    public class QuizQuestionRepository : IQuizQuestionRepository
    {
        private readonly CurriculumDbContext _dbContext;

        public QuizQuestionRepository(CurriculumDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<QuizQuestion>> GetQuestionsByQuizCodeAsync(string quizCode)
        {
            return await _dbContext.QuizQuestions
                .Where(q => q.QuizCode == quizCode)
                .ToListAsync();
        }

        public async Task<QuizQuestion?> GetQuestionByCodeAsync(string questionCode)
        {
            return await _dbContext.QuizQuestions
                .FirstOrDefaultAsync(q => q.QuizQuestionCode == questionCode);
        }

        public async Task<QuizQuestion> CreateQuestionAsync(QuizQuestionCreateDto questionDto)
        {
            // Validate that the quiz exists
            var quiz = await _dbContext.Quizzes.FirstOrDefaultAsync(q => q.QuizCode == questionDto.QuizCode);
            if (quiz == null)
            {
                throw new ArgumentException($"Quiz with code {questionDto.QuizCode} not found");
            }

            // Generate a unique question code
            var quizQuestionCode = await CodeGenerator.GenerateUniqueQuestionCode(_dbContext, questionDto.QuizCode);

            // Set question type based on quiz type
            string questionType = quiz.QuizType;

            var quizQuestion = new QuizQuestion
            {
                Points = questionDto.Points,
                QuizCode = questionDto.QuizCode,
                QuestionText = questionDto.QuestionText,
                QuizQuestionCode = quizQuestionCode,
                QuestionType = questionType // Set based on quiz type
            };

            _dbContext.QuizQuestions.Add(quizQuestion);
            await _dbContext.SaveChangesAsync();
            return quizQuestion;
        }

        public async Task<QuizQuestion> UpdateQuestionAsync(QuizQuestionUpdateDto questionDto)
        {
            var existingQuestion = await _dbContext.QuizQuestions
                .FirstOrDefaultAsync(q => q.QuizQuestionCode == questionDto.QuizQuestionCode);

            if (existingQuestion == null)
            {
                return null;
            }

            // Don't allow question code or quiz code to be changed
            questionDto.QuizQuestionCode = existingQuestion.QuizQuestionCode;
            questionDto.QuizCode = existingQuestion.QuizCode;

            // Update only allowed fields
            existingQuestion.QuestionText = questionDto.QuestionText;
            existingQuestion.Points = questionDto.Points;

            _dbContext.QuizQuestions.Update(existingQuestion);
            await _dbContext.SaveChangesAsync();
            return existingQuestion;
        }

        public async Task<bool> DeleteQuestionByCodeAsync(string questionCode)
        {
            var question = await _dbContext.QuizQuestions
                .FirstOrDefaultAsync(q => q.QuizQuestionCode == questionCode);

            if (question == null)
            {
                return false;
            }

            // Get all related answer options
            var answerOptions = await _dbContext.AnswerOptions
                .Where(a => a.QuizQuestionCode == questionCode)
                .ToListAsync();

            // Get all student answers for this question
            var studentAnswers = await _dbContext.StudentAnswers
                .Where(a => a.QuizQuestionCode == questionCode)
                .ToListAsync();

            // Remove related entities
            _dbContext.StudentAnswers.RemoveRange(studentAnswers);
            _dbContext.AnswerOptions.RemoveRange(answerOptions);
            _dbContext.QuizQuestions.Remove(question);

            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}