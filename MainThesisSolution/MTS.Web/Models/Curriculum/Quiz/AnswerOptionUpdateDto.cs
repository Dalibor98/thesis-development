using System.ComponentModel.DataAnnotations;

namespace MTS.Web.Models.Curriculum.Quiz
{
    public class AnswerOptionUpdateDto
    {
        public int Id { get; set; }
        public string QuizQuestionCode { get; set; }
        public string OptionCode { get; set; }

        [Required]
        [Display(Name = "Answer Text")]
        public string OptionText { get; set; }

        [Display(Name = "Is Correct Answer")]
        public bool IsCorrect { get; set; }
    }
}