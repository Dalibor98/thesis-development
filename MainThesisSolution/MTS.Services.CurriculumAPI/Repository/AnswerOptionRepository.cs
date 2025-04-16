using Microsoft.EntityFrameworkCore;
using MTS.Services.CurriculumAPI.Data;
using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO.AnswerOption;
using MTS.Services.CurriculumAPI.Repository.IRepository;
using MTS.Services.CurriculumAPI.Utilities;

namespace MTS.Services.CurriculumAPI.Repository
{
    public class AnswerOptionRepository : IAnswerOptionRepository
    {//CURRENT
        private readonly CurriculumDbContext _dbContext;

        public AnswerOptionRepository(CurriculumDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<AnswerOption>> GetOptionsByQuestionCodeAsync(string questionCode)
        {
            return await _dbContext.AnswerOptions
                .Where(a => a.QuizQuestionCode == questionCode)
                .ToListAsync();
        }

        public async Task<AnswerOption?> GetOptionByCodeAsync(string optionCode)
        {
            return await _dbContext.AnswerOptions
                .FirstOrDefaultAsync(a => a.OptionCode == optionCode);
        }

        public async Task<AnswerOption?> GetOptionByIdAsync(int id)
        {
            return await _dbContext.AnswerOptions.FindAsync(id);
        }

        public async Task<AnswerOption> CreateOptionAsync(AnswerOptionCreateDto optionDto)
        {
            // Validate that the question exists
            var question = await _dbContext.QuizQuestions
                .FirstOrDefaultAsync(q => q.QuizQuestionCode == optionDto.QuizQuestionCode);

            if (question == null)
            {
                throw new ArgumentException($"Question with code {optionDto.QuizQuestionCode} not found");
            }

            // Check if we're trying to create an option for a text-based question
            var quiz = await _dbContext.Quizzes
                .FirstOrDefaultAsync(q => q.QuizCode == question.QuizCode);

            if (quiz != null && quiz.QuizType == "TextBased")
            {
                throw new InvalidOperationException("Cannot add answer options to a text-based question");
            }

            // Generate a unique option code
            string optionCode = await CodeGenerator.GenerateUniqueOptionCode(_dbContext, optionDto.QuizQuestionCode);

            // Create the answer option
            var option = new AnswerOption
            {
                OptionCode = optionCode,
                QuizQuestionCode = optionDto.QuizQuestionCode,
                OptionText = optionDto.OptionText,
                IsCorrect = optionDto.IsCorrect
            };

            _dbContext.AnswerOptions.Add(option);
            await _dbContext.SaveChangesAsync();
            return option;
        }

        public async Task<AnswerOption> UpdateOptionAsync(AnswerOptionUpdateDto optionDto)
        {
            var existingOption = await _dbContext.AnswerOptions.FindAsync(optionDto.Id);
            if (existingOption == null)
            {
                throw new ArgumentException($"Answer option with ID {optionDto.Id} not found");
            }

            // Don't allow changing the question this option belongs to
            optionDto.QuizQuestionCode = existingOption.QuizQuestionCode;

            // Update properties
            existingOption.OptionText = optionDto.OptionText;
            existingOption.IsCorrect = optionDto.IsCorrect;

            _dbContext.AnswerOptions.Update(existingOption);
            await _dbContext.SaveChangesAsync();
            return existingOption;
        }

        public async Task<bool> DeleteOptionAsync(int id)
        {
            var option = await _dbContext.AnswerOptions.FindAsync(id);
            if (option == null)
            {
                return false;
            }

            // Get student answers that reference this option
            var studentAnswers = await _dbContext.StudentAnswers
                .Where(a => a.SelectedOptionCode == option.OptionCode)
                .ToListAsync();

            // Clear the option reference but don't delete student answers
            foreach (var studentAnswer in studentAnswers)
            {
                studentAnswer.SelectedOptionCode = null;
                studentAnswer.IsCorrect = false;
                studentAnswer.PointsEarned = 0;
            }

            _dbContext.AnswerOptions.Remove(option);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteOptionByCodeAsync(string optionCode)
        {
            var option = await _dbContext.AnswerOptions
                .FirstOrDefaultAsync(a => a.OptionCode == optionCode);

            if (option == null)
            {
                return false;
            }

            return await DeleteOptionAsync(option.Id);
        }

        public async Task<IEnumerable<AnswerOption>> GetCorrectOptionsForQuestionAsync(string questionCode)
        {
            return await _dbContext.AnswerOptions
                .Where(a => a.QuizQuestionCode == questionCode && a.IsCorrect)
                .ToListAsync();
        }
    }
}