using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MTS.Services.CurriculumAPI.Models;
using MTS.Services.CurriculumAPI.Models.DTO;
using MTS.Services.CurriculumAPI.Models.DTO.QuizDto;
using MTS.Services.CurriculumAPI.Repository.IRepository;

namespace MTS.Services.CurriculumAPI.Controllers
{
    [Route("api/quizzes")]
    [ApiController]
    public class QuizAPIController : ControllerBase
    {
        private readonly IQuizRepository _quizRepository;
        protected ResponseDto _response;

        public QuizAPIController(IQuizRepository quizRepository)
        {
            _quizRepository = quizRepository ?? throw new ArgumentNullException(nameof(quizRepository));
            _response = new ResponseDto();
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto>> GetQuizzes()
        {
            try
            {
                var quizzes = await _quizRepository.GetAllQuizzesAsync();
                _response.Result = quizzes;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ResponseDto>> GetQuizById(int id)
        {
            try
            {
                var quiz = await _quizRepository.GetQuizByIdAsync(id);
                if (quiz == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Quiz with ID {id} not found";
                    return NotFound(_response);
                }
                _response.Result = quiz;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("code/{quizCode}")]
        public async Task<ActionResult<ResponseDto>> GetQuizByCode(string quizCode)
        {
            try
            {
                var quiz = await _quizRepository.GetQuizByCodeAsync(quizCode);
                if (quiz == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Quiz with code {quizCode} not found";
                    return NotFound(_response);
                }
                _response.Result = quiz;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("course/{courseCode}")]
        public async Task<ActionResult<ResponseDto>> GetQuizzesByCourseCode(string courseCode)
        {
            try
            {
                var quizzes = await _quizRepository.GetQuizzesByCourseCodeAsync(courseCode);
                _response.Result = quizzes;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("week/{weekCode}")]
        public async Task<ActionResult<ResponseDto>> GetQuizzesByWeekCode(string weekCode)
        {
            try
            {
                var quizzes = await _quizRepository.GetQuizzesByWeekCodeAsync(weekCode);
                _response.Result = quizzes;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto>> CreateQuiz([FromBody] QuizCreateDto quizDto)
        {
            try
            {
                var createdQuiz = await _quizRepository.CreateQuizAsync(quizDto);
                _response.Result = createdQuiz;
                return CreatedAtAction(nameof(GetQuizByCode), new { quizCode = createdQuiz.QuizCode }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpPut]
        public async Task<ActionResult<ResponseDto>> UpdateQuiz([FromBody] QuizUpdateDto quizDto)
        {
            try
            {
                var updatedQuiz = await _quizRepository.UpdateQuizAsync(quizDto);
                if (updatedQuiz == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Quiz with code {quizDto.QuizCode} not found";
                    return NotFound(_response);
                }
                _response.Result = updatedQuiz;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ResponseDto>> DeleteQuiz(int id)
        {
            try
            {
                var result = await _quizRepository.DeleteQuizAsync(id);
                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Quiz with ID {id} not found";
                    return NotFound(_response);
                }
                _response.Result = result;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("{quizCode}/questions")]
        public async Task<ActionResult<ResponseDto>> GetQuestionsByQuizCode(string quizCode)
        {
            try
            {
                var questions = await _quizRepository.GetQuestionsByQuizCodeAsync(quizCode);
                _response.Result = questions;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("question/{questionCode}")]
        public async Task<ActionResult<ResponseDto>> GetQuestionByCode(string questionCode)
        {
            try
            {
                var question = await _quizRepository.GetQuestionByCodeAsync(questionCode);
                if (question == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Question with code {questionCode} not found";
                    return NotFound(_response);
                }
                _response.Result = question;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpPost("question")]
        public async Task<ActionResult<ResponseDto>> CreateQuestion([FromBody] QuizQuestionCreateDto questionDto)
        {
            try
            {
                var createdQuestion = await _quizRepository.CreateQuestionAsync(questionDto);
                _response.Result = createdQuestion;
                return CreatedAtAction(nameof(GetQuestionByCode), new { questionCode = createdQuestion.QuizQuestionCode }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpPut("question")]
        public async Task<ActionResult<ResponseDto>> UpdateQuestion([FromBody] QuizQuestionUpdateDto question)
        {
            try
            {
                var updatedQuestion = await _quizRepository.UpdateQuestionAsync(question);
                if (updatedQuestion == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Question with code {question.QuizQuestionCode} not found";
                    return NotFound(_response);
                }
                _response.Result = updatedQuestion;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpDelete("question/code/{questionCode}")]
        public async Task<ActionResult<ResponseDto>> DeleteQuestionByCode(string questionCode)
        {
            try
            {
                var result = await _quizRepository.DeleteQuestionByCodeAsync(questionCode);
                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Question with code {questionCode} not found";
                    return NotFound(_response);
                }
                _response.Result = result;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("question/{questionCode}/answers")]
        public async Task<ActionResult<ResponseDto>> GetAnswersByQuestionCode(string questionCode)
        {
            try
            {
                var answers = await _quizRepository.GetAnswersByQuestionCodeAsync(questionCode);
                _response.Result = answers;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpPost("answer")]
        public async Task<ActionResult<ResponseDto>> CreateAnswer([FromBody] Answer answer)
        {
            try
            {
                var createdAnswer = await _quizRepository.CreateAnswerAsync(answer);
                _response.Result = createdAnswer;
                return CreatedAtAction(nameof(GetAnswersByQuestionCode), new { questionCode = createdAnswer.QuizQuestionCode }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpPut("answer")]
        public async Task<ActionResult<ResponseDto>> UpdateAnswer([FromBody] Answer answer)
        {
            try
            {
                var updatedAnswer = await _quizRepository.UpdateAnswerAsync(answer);
                if (updatedAnswer == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Answer with ID {answer.Id} not found";
                    return NotFound(_response);
                }
                _response.Result = updatedAnswer;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpDelete("answer/{id:int}")]
        public async Task<ActionResult<ResponseDto>> DeleteAnswer(int id)
        {
            try
            {
                var result = await _quizRepository.DeleteAnswerAsync(id);
                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Answer with ID {id} not found";
                    return NotFound(_response);
                }
                _response.Result = result;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("attempts/quiz/{quizCode}")]
        public async Task<ActionResult<ResponseDto>> GetAttemptsByQuizCode(string quizCode)
        {
            try
            {
                var attempts = await _quizRepository.GetAttemptsByQuizCodeAsync(quizCode);
                _response.Result = attempts;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("attempts/student/{studentId}")]
        public async Task<ActionResult<ResponseDto>> GetAttemptsByStudentId(string studentId)
        {
            try
            {
                var attempts = await _quizRepository.GetAttemptsByStudentIdAsync(studentId);
                _response.Result = attempts;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("attempt/{attemptCode}")]
        public async Task<ActionResult<ResponseDto>> GetAttemptByCode(string attemptCode)
        {
            try
            {
                var attempt = await _quizRepository.GetAttemptByCodeAsync(attemptCode);
                if (attempt == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Attempt with code {attemptCode} not found";
                    return NotFound(_response);
                }
                _response.Result = attempt;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpPost("attempt")]
        public async Task<ActionResult<ResponseDto>> CreateAttempt([FromBody] StudentQuizAttemptCreateDto attempt)
        {
            try
            {
                var createdAttempt = await _quizRepository.CreateAttemptAsync(attempt);
                _response.Result = createdAttempt;
                return CreatedAtAction(nameof(GetAttemptByCode), new { attemptCode = createdAttempt.AttemptCode }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpPut("attempt")]
        public async Task<ActionResult<ResponseDto>> UpdateAttempt([FromBody] StudentQuizAttempt attempt)
        {
            try
            {
                var updatedAttempt = await _quizRepository.UpdateAttemptAsync(attempt);
                if (updatedAttempt == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Attempt with ID {attempt.Id} not found";
                    return NotFound(_response);
                }
                _response.Result = updatedAttempt;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("attempt/{attemptCode}/answers")]
        public async Task<ActionResult<ResponseDto>> GetStudentAnswersByAttemptCode(string attemptCode)
        {
            try
            {
                var answers = await _quizRepository.GetAnswersByAttemptCodeAsync(attemptCode);
                _response.Result = answers;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpPost("studentanswer")]
        public async Task<ActionResult<ResponseDto>> CreateStudentAnswer([FromBody] StudentQuizAnswerCreateDto answer)
        {
            try
            {
                var createdAnswer = await _quizRepository.CreateStudentAnswerAsync(answer);
                _response.Result = createdAnswer;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpPut("studentanswer")]
        public async Task<ActionResult<ResponseDto>> UpdateStudentAnswer([FromBody] StudentQuizAnswer answer)
        {
            try
            {
                var updatedAnswer = await _quizRepository.UpdateStudentAnswerAsync(answer);
                if (updatedAnswer == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Student answer with ID {answer.Id} not found";
                    return NotFound(_response);
                }
                _response.Result = updatedAnswer;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }
        [HttpGet("attempt/{attemptCode}/score")]
        public async Task<ActionResult<ResponseDto>> CalculateScore(string attemptCode)
        {
            try
            {
                var attempt = await _quizRepository.GetAttemptByCodeAsync(attemptCode);
                if (attempt == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Attempt with code {attemptCode} not found";
                    return NotFound(_response);
                }

                // Get the quiz to determine its type
                var quiz = await _quizRepository.GetQuizByCodeAsync(attempt.QuizCode);
                if (quiz == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Quiz not found for attempt {attemptCode}";
                    return NotFound(_response);
                }

                // Get all questions for this quiz
                var questions = await _quizRepository.GetQuestionsByQuizCodeAsync(attempt.QuizCode);

                // Get student answers
                var studentAnswers = await _quizRepository.GetAnswersByAttemptCodeAsync(attemptCode);

                // Calculate total possible points
                int totalPossible = questions.Sum(q => q.Points);
                int totalEarned = 0;

                if (quiz.QuizType == "MultipleChoice")
                {
                    // For multiple choice, we can automatically grade
                    foreach (var studentAnswer in studentAnswers)
                    {
                        // Find the corresponding question
                        var question = questions.FirstOrDefault(q => q.QuizQuestionCode == studentAnswer.QuizQuestionCode);
                        if (question == null) continue;

                        // If the student selected an answer
                        if (!string.IsNullOrEmpty(studentAnswer.AnswerCode))
                        {
                            // Get the correct answer for this question
                            var correctAnswers = await _quizRepository.GetAnswersByQuestionCodeAsync(studentAnswer.QuizQuestionCode);
                            var correctAnswer = correctAnswers.FirstOrDefault(a => a.IsCorrect);

                            // If the student selected the correct answer
                            if (correctAnswer != null && studentAnswer.AnswerCode == correctAnswer.AnswerCode)
                            {
                                studentAnswer.IsCorrect = true;
                                studentAnswer.PointsEarned = question.Points;
                                totalEarned += question.Points;
                            }
                            else
                            {
                                studentAnswer.IsCorrect = false;
                                studentAnswer.PointsEarned = 0;
                            }

                            // Update the student answer
                            await _quizRepository.UpdateStudentAnswerAsync(studentAnswer);
                        }
                    }
                }
                else // Text-based quiz
                {
                    // For text-based quizzes, we need to check if they've been manually graded
                    // If any answers have been manually graded, use those points
                    foreach (var studentAnswer in studentAnswers)
                    {
                        totalEarned += studentAnswer.PointsEarned;
                    }
                }

                // Calculate the percentage score (0-100)
                int score = totalPossible > 0 ? (int)Math.Round((double)totalEarned / totalPossible * 100) : 0;

                // Update the attempt with the score
                attempt.Score = score;
                await _quizRepository.UpdateAttemptAsync(attempt);

                _response.Result = score;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("professor/{professorId}/attempts")]
        public async Task<ActionResult<ResponseDto>> GetRecentQuizAttemptsByProfessorId(string professorId)
        {
            try
            {
                var attempts = await _quizRepository.GetRecentAttemptsByProfessorIdAsync(professorId);
                _response.Result = attempts;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("student/{studentId}/upcoming")]
        public async Task<ActionResult<ResponseDto>> GetUpcomingQuizzesByStudentId(string studentId)
        {
            try
            {
                var quizzes = await _quizRepository.GetUpcomingQuizzesByStudentIdAsync(studentId);

                _response.Result = quizzes;
                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;

                return StatusCode(500, _response);
            }
        }

        [HttpPost("studentanswer/grade")]
        public async Task<ActionResult<ResponseDto>> GradeStudentAnswer([FromBody] StudentQuizAnswerGradeDto gradeDto)
        {
            try
            {
                var studentAnswer = await _quizRepository.GradeStudentAnswerAsync(gradeDto);

                if (studentAnswer == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Student answer with ID {gradeDto.Id} not found";
                    return NotFound(_response);
                }

                _response.Result = studentAnswer;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("studentanswer/{id:int}")]
        public async Task<ActionResult<ResponseDto>> GetStudentAnswerById(int id)
        {
            try
            {
                var answer = await _quizRepository.GetStudentAnswerByIdAsync(id);
                if (answer == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Student answer with ID {id} not found";
                    return NotFound(_response);
                }
                _response.Result = answer;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpGet("answer/{answerCode}")]
        public async Task<ActionResult<ResponseDto>> GetAnswerByCode(string answerCode)
        {
            try
            {
                var answers = await _quizRepository.GetAnswersByQuestionCodeAsync(answerCode);
                var answer = answers.FirstOrDefault(a => a.AnswerCode == answerCode);

                if (answer == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Answer with code {answerCode} not found";
                    return NotFound(_response);
                }

                _response.Result = answer;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }

    }
}