using System.ComponentModel.DataAnnotations;

namespace MTS.Web.Models.Curriculum.Quiz
{
    public class QuizQuestionUpdateDto
    {
        [Required]
        public string QuizCode { get; set; }

        [Required]
        public string QuizQuestionCode { get; set; }

        [Required]
        [Display(Name = "Question Text")]
        public string QuestionText { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Points must be between 1 and 100")]
        [Display(Name = "Points")]
        public int Points { get; set; }
    }
}
