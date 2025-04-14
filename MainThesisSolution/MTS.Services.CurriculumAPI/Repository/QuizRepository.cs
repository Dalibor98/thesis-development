using Microsoft.EntityFrameworkCore;
using MTS.Services.CurriculumAPI.Data;
using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO;
using MTS.Services.CurriculumAPI.Models.DTO.QuizDto;
using MTS.Services.CurriculumAPI.Repository.IRepository;
using MTS.Services.CurriculumAPI.Utilities;

namespace MTS.Services.CurriculumAPI.Repository
{
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
            var quiz = await _dbContext.Quizzes.FindAsync(id);
            if (quiz == null)
            {
                return false;
            }

            // Get all related entities
            var questions = await _dbContext.QuizQuestions
                .Where(q => q.QuizCode == quiz.QuizCode)
                .ToListAsync();

            var questionCodes = questions.Select(q => q.QuizQuestionCode).ToList();

            var answers = await _dbContext.Answers
                .Where(a => questionCodes.Contains(a.QuizQuestionCode))
                .ToListAsync();

            var attempts = await _dbContext.StudentQuizAttempts
                .Where(a => a.QuizCode == quiz.QuizCode)
                .ToListAsync();

            var attemptCodes = attempts.Select(a => a.AttemptCode).ToList();

            var studentAnswers = await _dbContext.StudentQuizAnswers
                .Where(a => attemptCodes.Contains(a.AttemptCode))
                .ToListAsync();

            // Remove all related entities
            _dbContext.StudentQuizAnswers.RemoveRange(studentAnswers);
            _dbContext.StudentQuizAttempts.RemoveRange(attempts);
            _dbContext.Answers.RemoveRange(answers);
            _dbContext.QuizQuestions.RemoveRange(questions);
            _dbContext.Quizzes.Remove(quiz);

            await _dbContext.SaveChangesAsync();
            return true;
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

        public async Task<QuizQuestion> CreateQuestionAsync(QuizQuestionCreateDto question)
        {
            var quizQuestionCode = await CodeGenerator.GenerateUniqueQuestionCode(_dbContext, question.QuizCode);

            var quizQuestion = new QuizQuestion
            {
                Points = question.Points,
                QuizCode = question.QuizCode,
                QuestionText = question.QuestionText,
                QuizQuestionCode = quizQuestionCode
            };
            

            _dbContext.QuizQuestions.Add(quizQuestion);
            await _dbContext.SaveChangesAsync();
            return quizQuestion;
        }

        public async Task<QuizQuestion> UpdateQuestionAsync(QuizQuestionUpdateDto question)
        {
            var existingQuestion = await _dbContext.QuizQuestions.FirstOrDefaultAsync(q =>  q.QuizQuestionCode == question.QuizQuestionCode);

            if (existingQuestion == null)
            {
                return null;
            }

            // Don't allow question code or quiz code to be changed
            question.QuizQuestionCode = existingQuestion.QuizQuestionCode;
            question.QuizCode = existingQuestion.QuizCode;

            _dbContext.Entry(existingQuestion).CurrentValues.SetValues(question);
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

            // Get all related answers
            var answers = await _dbContext.Answers
                .Where(a => a.QuizQuestionCode == questionCode)
                .ToListAsync();

            // Get all student answers for this question
            var studentAnswers = await _dbContext.StudentQuizAnswers
                .Where(a => a.QuizQuestionCode == questionCode)
                .ToListAsync();

            // Remove related entities
            _dbContext.StudentQuizAnswers.RemoveRange(studentAnswers);
            _dbContext.Answers.RemoveRange(answers);
            _dbContext.QuizQuestions.Remove(question);

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Answer>> GetAnswersByQuestionCodeAsync(string questionCode)
        {
            return await _dbContext.Answers
                .Where(a => a.QuizQuestionCode == questionCode)
                .ToListAsync();
        }

        public async Task<Answer> CreateAnswerAsync(Answer answer)
        {
            // Generate answer code if not provided
            if (string.IsNullOrEmpty(answer.AnswerCode))
            {
                answer.AnswerCode = await CodeGenerator.GenerateUniqueAnswerCode(_dbContext, answer.QuizQuestionCode);
            }

            _dbContext.Answers.Add(answer);
            await _dbContext.SaveChangesAsync();
            return answer;
        }

        public async Task<Answer> UpdateAnswerAsync(Answer answer)
        {
            var existingAnswer = await _dbContext.Answers.FindAsync(answer.Id);
            if (existingAnswer == null)
            {
                return null;
            }

            // Don't allow answer code or question code to be changed
            answer.AnswerCode = existingAnswer.AnswerCode;
            answer.QuizQuestionCode = existingAnswer.QuizQuestionCode;

            _dbContext.Entry(existingAnswer).CurrentValues.SetValues(answer);
            await _dbContext.SaveChangesAsync();
            return existingAnswer;
        }

        public async Task<bool> DeleteAnswerAsync(int id)
        {
            var answer = await _dbContext.Answers.FindAsync(id);
            if (answer == null)
            {
                return false;
            }

            // Get all student answers using this answer
            var studentAnswers = await _dbContext.StudentQuizAnswers
                .Where(a => a.AnswerCode == answer.AnswerCode)
                .ToListAsync();

            // Clear the answer reference but don't delete student answers
            foreach (var studentAnswer in studentAnswers)
            {
                studentAnswer.AnswerCode = null;
                studentAnswer.IsCorrect = false;
            }

            _dbContext.Answers.Remove(answer);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<StudentQuizAttempt>> GetAttemptsByQuizCodeAsync(string quizCode)
        {
            return await _dbContext.StudentQuizAttempts
                .Where(a => a.QuizCode == quizCode)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentQuizAttempt>> GetAttemptsByStudentIdAsync(string studentUniversityId)
        {
            return await _dbContext.StudentQuizAttempts
                .Where(a => a.StudentUniversityId == studentUniversityId)
                .ToListAsync();
        }

        public async Task<StudentQuizAttempt?> GetAttemptByCodeAsync(string attemptCode)
        {
            return await _dbContext.StudentQuizAttempts
                .FirstOrDefaultAsync(a => a.AttemptCode == attemptCode);
        }

        public async Task<StudentQuizAttempt> CreateAttemptAsync(StudentQuizAttemptCreateDto attemptDto)
        {
            var quizAttemptCode = await CodeGenerator.GenerateUniqueAttemptCode(_dbContext, attemptDto.QuizCode, attemptDto.StudentUniversityId);

            var attempt = new StudentQuizAttempt
            {
                AttemptCode = quizAttemptCode,
                EndTime = attemptDto.EndTime,
                StartTime = attemptDto.StartTime,
                Score = attemptDto.Score,
                StudentUniversityId = attemptDto.StudentUniversityId,
                QuizCode = attemptDto.QuizCode
            };

            _dbContext.StudentQuizAttempts.Add(attempt);
            await _dbContext.SaveChangesAsync();
            return attempt;
        }

        public async Task<StudentQuizAttempt> UpdateAttemptAsync(StudentQuizAttempt attempt)
        {
            var existingAttempt = await _dbContext.StudentQuizAttempts.FindAsync(attempt.Id);
            if (existingAttempt == null)
            {
                return null;
            }

            // Don't allow attempt code, quiz code, or student ID to be changed
            attempt.AttemptCode = existingAttempt.AttemptCode;
            attempt.QuizCode = existingAttempt.QuizCode;
            attempt.StudentUniversityId = existingAttempt.StudentUniversityId;

            _dbContext.Entry(existingAttempt).CurrentValues.SetValues(attempt);
            await _dbContext.SaveChangesAsync();
            return existingAttempt;
        }

        public async Task<IEnumerable<StudentQuizAnswer>> GetAnswersByAttemptCodeAsync(string attemptCode)
        {
            return await _dbContext.StudentQuizAnswers
                .Where(a => a.AttemptCode == attemptCode)
                .ToListAsync();
        }

        public async Task<StudentQuizAnswer> CreateStudentAnswerAsync(StudentQuizAnswerCreateDto answerDto)
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
            var existingAnswer = await _dbContext.StudentQuizAnswers
                .FirstOrDefaultAsync(a => a.AttemptCode == answerDto.AttemptCode &&
                                      a.QuizQuestionCode == answerDto.QuizQuestionCode);

            if (existingAnswer != null)
            {
                // Update the existing answer
                existingAnswer.AnswerCode = answerDto.AnswerCode;
                existingAnswer.TextAnswer = answerDto.TextAnswer;

                // Calculate if the answer is correct and the points earned
                await DetermineAnswerCorrectness(existingAnswer, question);

                _dbContext.StudentQuizAnswers.Update(existingAnswer);
                await _dbContext.SaveChangesAsync();
                return existingAnswer;
            }
            else
            {
                // Create a new student answer
                var answer = new StudentQuizAnswer
                {
                    AttemptCode = answerDto.AttemptCode,
                    QuizQuestionCode = answerDto.QuizQuestionCode,
                    AnswerCode = answerDto.AnswerCode,
                    TextAnswer = answerDto.TextAnswer,
                    IsCorrect = false, // Default value, will be updated
                    PointsEarned = 0   // Default value, will be updated
                };

                // Calculate if the answer is correct and the points earned
                await DetermineAnswerCorrectness(answer, question);

                _dbContext.StudentQuizAnswers.Add(answer);
                await _dbContext.SaveChangesAsync();
                return answer;
            }
        }

        // Helper method to determine if an answer is correct and calculate points
        // Helper method to determine if an answer is correct and calculate points
        private async Task DetermineAnswerCorrectness(StudentQuizAnswer studentAnswer, QuizQuestion question)
        {
            // First get the question to get the quiz code
            var questionEntity = await _dbContext.QuizQuestions
                .FirstOrDefaultAsync(qq => qq.QuizQuestionCode == question.QuizQuestionCode);

            // Then get the quiz to determine its type
            var quiz = questionEntity != null ?
                await _dbContext.Quizzes.FirstOrDefaultAsync(q => q.QuizCode == questionEntity.QuizCode) : null;

            // Handle based on quiz type
            if (quiz?.QuizType == "MultipleChoice")
            {
                // For multiple-choice questions, check if the selected answer is correct
                if (!string.IsNullOrEmpty(studentAnswer.AnswerCode))
                {
                    var selectedAnswer = await _dbContext.Answers
                        .FirstOrDefaultAsync(a => a.AnswerCode == studentAnswer.AnswerCode);

                    if (selectedAnswer != null)
                    {
                        // Use the IsCorrect flag from the answer option
                        studentAnswer.IsCorrect = selectedAnswer.IsCorrect;
                        studentAnswer.PointsEarned = selectedAnswer.IsCorrect ? question.Points : 0;
                    }
                    else
                    {
                        // Answer option not found (should not happen in normal flow)
                        studentAnswer.IsCorrect = false;
                        studentAnswer.PointsEarned = 0;
                    }
                }
                else
                {
                    // No answer provided
                    studentAnswer.IsCorrect = false;
                    studentAnswer.PointsEarned = 0;
                }
            }
            else if (quiz?.QuizType == "TextBased")
            {
                // For text/essay questions, these need manual grading
                // Leave IsCorrect as false and PointsEarned as 0 for now
                if (!string.IsNullOrEmpty(studentAnswer.TextAnswer))
                {
                    studentAnswer.IsCorrect = false;  // Will be set by professor during grading
                    studentAnswer.PointsEarned = 0;   // Will be set by professor during grading
                }
                else
                {
                    // No answer provided
                    studentAnswer.IsCorrect = false;
                    studentAnswer.PointsEarned = 0;
                }
            }
            else
            {
                // Default case
                studentAnswer.IsCorrect = false;
                studentAnswer.PointsEarned = 0;
            }
        }

        public async Task<StudentQuizAnswer> UpdateStudentAnswerAsync(StudentQuizAnswer answer)
        {
            var existingAnswer = await _dbContext.StudentQuizAnswers.FindAsync(answer.Id);
            if (existingAnswer == null)
            {
                return null;
            }

            // Don't allow attempt code or question code to be changed
            answer.AttemptCode = existingAnswer.AttemptCode;
            answer.QuizQuestionCode = existingAnswer.QuizQuestionCode;

            _dbContext.Entry(existingAnswer).CurrentValues.SetValues(answer);
            await _dbContext.SaveChangesAsync();
            return existingAnswer;
        }

        public async Task<IEnumerable<StudentQuizAttempt>> GetRecentAttemptsByProfessorIdAsync(string professorId)
        {
            // 1. Find all courses for this professor
            var courses = await _dbContext.Courses
                .Where(c => c.ProfessorUniversityId == professorId)
                .ToListAsync();

            if (!courses.Any())
            {
                return new List<StudentQuizAttempt>();
            }

            // 2. Get all course codes
            var courseCodes = courses.Select(c => c.CourseCode).ToList();

            // 3. Find all quizzes in these courses
            var quizzes = await _dbContext.Quizzes
                .Where(q => courseCodes.Contains(q.CourseCode))
                .ToListAsync();

            if (!quizzes.Any())
            {
                return new List<StudentQuizAttempt>();
            }

            // 4. Get all quiz codes
            var quizCodes = quizzes.Select(q => q.QuizCode).ToList();

            // 5. Find all attempts for these quizzes
            var attempts = await _dbContext.StudentQuizAttempts
                .Where(a => quizCodes.Contains(a.QuizCode))
                // Order by most recent first
                .OrderByDescending(a => a.EndTime)
                // Limit to the most recent 20 attempts
                .Take(20)
                .ToListAsync();

            return attempts;
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

        // MTS.Services.CurriculumAPI/Repository/QuizRepository.cs - Implement the method
        public async Task<StudentQuizAnswer> GradeStudentAnswerAsync(StudentQuizAnswerGradeDto gradeDto)
        {
            var studentAnswer = await _dbContext.StudentQuizAnswers.FindAsync(gradeDto.Id);
            if (studentAnswer == null)
            {
                return null;
            }

            // Update the grading
            studentAnswer.IsCorrect = gradeDto.IsCorrect;
            studentAnswer.PointsEarned = gradeDto.PointsEarned;

            _dbContext.StudentQuizAnswers.Update(studentAnswer);
            await _dbContext.SaveChangesAsync();

            // Get the attempt to recalculate the score
            var attempt = await _dbContext.StudentQuizAttempts
                .FirstOrDefaultAsync(a => a.AttemptCode == studentAnswer.AttemptCode);

            if (attempt != null)
            {
                // Get all questions for this quiz
                var questions = await GetQuestionsByQuizCodeAsync(attempt.QuizCode);

                // Get student answers
                var studentAnswers = await GetAnswersByAttemptCodeAsync(attempt.AttemptCode);

                // Calculate total possible points
                int totalPossible = questions.Sum(q => q.Points);
                int totalEarned = studentAnswers.Sum(a => a.PointsEarned);

                // Calculate the percentage score (0-100)
                int score = totalPossible > 0 ? (int)Math.Round((double)totalEarned / totalPossible * 100) : 0;

                // Update the attempt with the score
                attempt.Score = score;
                await UpdateAttemptAsync(attempt);
            }

            return studentAnswer;
        }
        public async Task<StudentQuizAnswer?> GetStudentAnswerByIdAsync(int id)
        {
            return await _dbContext.StudentQuizAnswers.FindAsync(id);
        }
    }
}