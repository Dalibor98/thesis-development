using System.ComponentModel.DataAnnotations;

namespace MTS.Services.CurriculumAPI.Models.DTO.QuizDto
{
    public class StudentQuizAnswerCreateDto
    {
        public string AttemptCode { get; set; }
        public string QuizQuestionCode { get; set; }
        public string AnswerCode { get; set; }
        public string TextAnswer { get; set; }
    }
}
