using System.ComponentModel.DataAnnotations;

namespace MTS.Web.Models.Curriculum.Quiz
{
    public class StudentQuizAnswerCreateDto
    {
        [Required]
        public string AttemptCode { get; set; }

        [Required]
        public string QuizQuestionCode { get; set; }

        public string TextAnswer { get; set; }
    }
}
