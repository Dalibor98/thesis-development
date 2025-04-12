using System.ComponentModel.DataAnnotations;

namespace MTS.Web.Models.Curriculum.Quiz
{
    public class AnswerCreateDto
    {
        [Required]
        public string QuizQuestionCode { get; set; }

        [Required]
        [Display(Name = "Answer Text")]
        public string OptionText { get; set; }

        [Display(Name = "Is Correct Answer")]
        public bool IsCorrect { get; set; }
    }
}
