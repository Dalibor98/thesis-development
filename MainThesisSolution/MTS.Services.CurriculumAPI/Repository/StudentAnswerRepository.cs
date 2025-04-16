using Microsoft.EntityFrameworkCore;
using MTS.Services.CurriculumAPI.Data;
using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO.QuizDto;
using MTS.Services.CurriculumAPI.Models.DTO.StudentAnswer;
using MTS.Services.CurriculumAPI.Models.DTO.StudentAnswerDto;
using MTS.Services.CurriculumAPI.Repository.IRepository;

namespace MTS.Services.CurriculumAPI.Repository
{//CURRENT
    public class StudentAnswerRepository : IStudentAnswerRepository
    {
        private readonly CurriculumDbContext _dbContext;

        public StudentAnswerRepository(CurriculumDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<StudentAnswer>> GetAnswersByAttemptCodeAsync(string attemptCode)
        {
            return await _dbContext.StudentAnswers
                .Where(a => a.AttemptCode == attemptCode)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentAnswer>> GetAnswersByQuestionCodeAsync(string questionCode)
        {
            return await _dbContext.StudentAnswers
                .Where(a => a.QuizQuestionCode == questionCode)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentAnswer>> GetAnswersByStudentIdAsync(string studentUniversityId)
        {
            // First get all attempts by this student
            var attempts = await _dbContext.StudentQuizAttempts
                .Where(a => a.StudentUniversityId == studentUniversityId)
                .ToListAsync();

            if (!attempts.Any())
                return new List<StudentAnswer>();

            // Then get all answers for these attempts
            var attemptCodes = attempts.Select(a => a.AttemptCode).ToList();
            return await _dbContext.StudentAnswers
                .Where(a => attemptCodes.Contains(a.AttemptCode))
                .ToListAsync();
        }

        public async Task<StudentAnswer?> GetAnswerByIdAsync(int id)
        {
            return await _dbContext.StudentAnswers.FindAsync(id);
        }

        public async Task<StudentAnswer> CreateStudentAnswerAsync(StudentAnswerCreateDto answerDto)
        {
            // Validate that the attempt exists
            var attempt = await _dbContext.StudentQuizAttempts
                .FirstOrDefaultAsync(a => a.AttemptCode == answerDto.AttemptCode);

            if (attempt == null)
            {
                throw new ArgumentException($"Quiz attempt with code {answerDto.AttemptCode} not found");
            }

            // Validate that the question exists
            var question = await _dbContext.QuizQuestions
                .FirstOrDefaultAsync(q => q.QuizQuestionCode == answerDto.QuizQuestionCode);

            if (question == null)
            {
                throw new ArgumentException($"Question with code {answerDto.QuizQuestionCode} not found");
            }

            // Check if the student already answered this question in this attempt
            var existingAnswer = await _dbContext.StudentAnswers
                .FirstOrDefaultAsync(a => a.AttemptCode == answerDto.AttemptCode &&
                                       a.QuizQuestionCode == answerDto.QuizQuestionCode);

            if (existingAnswer != null)
            {
                // Update the existing answer
                existingAnswer.SelectedOptionCode = answerDto.SelectedOptionCode;
                existingAnswer.TextAnswer = answerDto.TextAnswer;

                // Calculate if the answer is correct and the points earned
                await DetermineAnswerCorrectness(existingAnswer, question);

                _dbContext.StudentAnswers.Update(existingAnswer);
                await _dbContext.SaveChangesAsync();
                return existingAnswer;
            }
            else
            {
                // Create a new student answer
                var answer = new StudentAnswer
                {
                    AttemptCode = answerDto.AttemptCode,
                    QuizQuestionCode = answerDto.QuizQuestionCode,
                    SelectedOptionCode = answerDto.SelectedOptionCode,
                    TextAnswer = answerDto.TextAnswer,
                    IsCorrect = false, // Default value, will be updated
                    PointsEarned = 0,   // Default value, will be updated
                    GradingStatus = "Ungraded" // Default value, will be updated
                };

                // Calculate if the answer is correct and the points earned
                await DetermineAnswerCorrectness(answer, question);

                _dbContext.StudentAnswers.Add(answer);
                await _dbContext.SaveChangesAsync();
                return answer;
            }
        }

        public async Task<StudentAnswer> UpdateStudentAnswerAsync(StudentAnswerUpdateDto updateDto)
        {
            var existingAnswer = await _dbContext.StudentAnswers.FindAsync(updateDto.Id);
            if (existingAnswer == null)
            {
                throw new ArgumentException($"Student answer with ID {updateDto.Id} not found");
            }

            // Don't allow changing the attempt code or question code 
            // (these fields aren't in the DTO but we're making this explicit)

            // Update the modifiable fields
            existingAnswer.SelectedOptionCode = updateDto.SelectedOptionCode;
            existingAnswer.TextAnswer = updateDto.TextAnswer;
            existingAnswer.IsCorrect = updateDto.IsCorrect;
            existingAnswer.PointsEarned = updateDto.PointsEarned;
            existingAnswer.GradingStatus = updateDto.GradingStatus;

            // If this is a multiple-choice answer and the selected option changed,
            // we may need to recalculate correctness
            if (!string.IsNullOrEmpty(updateDto.SelectedOptionCode) &&
                updateDto.SelectedOptionCode != existingAnswer.SelectedOptionCode)
            {
                var question = await _dbContext.QuizQuestions
                    .FirstOrDefaultAsync(q => q.QuizQuestionCode == existingAnswer.QuizQuestionCode);

                if (question != null)
                {
                    await DetermineAnswerCorrectness(existingAnswer, question);
                }
            }

            _dbContext.StudentAnswers.Update(existingAnswer);
            await _dbContext.SaveChangesAsync();

            return existingAnswer;
        }

        public async Task<StudentAnswer> GradeStudentAnswerAsync(StudentAnswerGradeDto gradeDto)
        {
            var studentAnswer = await _dbContext.StudentAnswers.FindAsync(gradeDto.Id);
            if (studentAnswer == null)
            {
                throw new ArgumentException($"Student answer with ID {gradeDto.Id} not found");
            }

            // Update the grading
            studentAnswer.IsCorrect = gradeDto.IsCorrect;
            studentAnswer.PointsEarned = gradeDto.PointsEarned;
            studentAnswer.GradingStatus = "ManuallyGraded";

            _dbContext.StudentAnswers.Update(studentAnswer);
            await _dbContext.SaveChangesAsync();

            // Get the attempt to recalculate the score
            var attempt = await _dbContext.StudentQuizAttempts
                .FirstOrDefaultAsync(a => a.AttemptCode == studentAnswer.AttemptCode);

            if (attempt != null)
            {
                // Get all questions for this quiz
                var questions = await _dbContext.QuizQuestions
                    .Where(q => q.QuizCode == attempt.QuizCode)
                    .ToListAsync();

                // Get student answers
                var studentAnswers = await GetAnswersByAttemptCodeAsync(attempt.AttemptCode);

                // Calculate total possible points
                int totalPossible = questions.Sum(q => q.Points);
                int totalEarned = studentAnswers.Sum(a => a.PointsEarned);

                // Calculate the percentage score (0-100)
                int score = totalPossible > 0 ? (int)Math.Round((double)totalEarned / totalPossible * 100) : 0;

                // Update the attempt with the score
                attempt.Score = score;
                _dbContext.StudentQuizAttempts.Update(attempt);
                await _dbContext.SaveChangesAsync();
            }

            return studentAnswer;
        }

        public async Task<IEnumerable<StudentAnswer>> GetUngradedAnswersAsync(string professorId)
        {
            // First get all courses by this professor
            var courses = await _dbContext.Courses
                .Where(c => c.ProfessorUniversityId == professorId)
                .ToListAsync();

            if (!courses.Any())
                return new List<StudentAnswer>();

            // Get course codes
            var courseCodes = courses.Select(c => c.CourseCode).ToList();

            // Get all text-based quizzes in these courses
            var quizzes = await _dbContext.Quizzes
                .Where(q => courseCodes.Contains(q.CourseCode) && q.QuizType == "TextBased")
                .ToListAsync();

            if (!quizzes.Any())
                return new List<StudentAnswer>();

            // Get quiz codes
            var quizCodes = quizzes.Select(q => q.QuizCode).ToList();

            // Get all text-based answers that need grading
            // 1. Get all attempts for these quizzes
            var attempts = await _dbContext.StudentQuizAttempts
                .Where(a => quizCodes.Contains(a.QuizCode))
                .ToListAsync();

            if (!attempts.Any())
                return new List<StudentAnswer>();

            // 2. Get all answers for these attempts with text answers and "Ungraded" status
            var attemptCodes = attempts.Select(a => a.AttemptCode).ToList();
            return await _dbContext.StudentAnswers
                .Where(a => attemptCodes.Contains(a.AttemptCode)
                         && !string.IsNullOrEmpty(a.TextAnswer)
                         && a.GradingStatus == "Ungraded")
                .ToListAsync();
        }

        public async Task<bool> DeleteStudentAnswerAsync(int id)
        {
            var answer = await _dbContext.StudentAnswers.FindAsync(id);
            if (answer == null)
            {
                return false;
            }

            _dbContext.StudentAnswers.Remove(answer);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Helper method to determine if an answer is correct and calculate points
        private async Task DetermineAnswerCorrectness(StudentAnswer studentAnswer, QuizQuestion question)
        {
            // Get the quiz to determine its type
            var quiz = await _dbContext.Quizzes
                .FirstOrDefaultAsync(q => q.QuizCode == question.QuizCode);

            // Handle based on quiz type
            if (quiz?.QuizType == "MultipleChoice")
            {
                // For multiple-choice questions, check if the selected answer is correct
                if (!string.IsNullOrEmpty(studentAnswer.SelectedOptionCode))
                {
                    var selectedOption = await _dbContext.AnswerOptions
                        .FirstOrDefaultAsync(a => a.OptionCode == studentAnswer.SelectedOptionCode);

                    if (selectedOption != null)
                    {
                        // Use the IsCorrect flag from the answer option
                        studentAnswer.IsCorrect = selectedOption.IsCorrect;
                        studentAnswer.PointsEarned = selectedOption.IsCorrect ? question.Points : 0;
                        studentAnswer.GradingStatus = "AutoGraded";
                    }
                    else
                    {
                        // Answer option not found (should not happen in normal flow)
                        studentAnswer.IsCorrect = false;
                        studentAnswer.PointsEarned = 0;
                        studentAnswer.GradingStatus = "AutoGraded";
                    }
                }
                else
                {
                    // No answer provided
                    studentAnswer.IsCorrect = false;
                    studentAnswer.PointsEarned = 0;
                    studentAnswer.GradingStatus = "AutoGraded";
                }
            }
            else if (quiz?.QuizType == "TextBased")
            {
                // For text/essay questions, these need manual grading
                if (!string.IsNullOrEmpty(studentAnswer.TextAnswer))
                {
                    studentAnswer.IsCorrect = false;  // Will be set by professor during grading
                    studentAnswer.PointsEarned = 0;   // Will be set by professor during grading
                    studentAnswer.GradingStatus = "Ungraded";
                }
                else
                {
                    // No answer provided
                    studentAnswer.IsCorrect = false;
                    studentAnswer.PointsEarned = 0;
                    studentAnswer.GradingStatus = "Ungraded";
                }
            }
            else
            {
                // Default case
                studentAnswer.IsCorrect = false;
                studentAnswer.PointsEarned = 0;
                studentAnswer.GradingStatus = "Ungraded";
            }
        }
    }
}