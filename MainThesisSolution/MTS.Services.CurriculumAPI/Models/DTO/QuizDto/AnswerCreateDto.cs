using System.ComponentModel.DataAnnotations;

namespace MTS.Services.CurriculumAPI.Models.DTO.QuizDto
{
    public class AnswerCreateDto
    {   
        public string QuizQuestionCode { get; set; }
        public string OptionText { get; set; }
        public bool IsCorrect { get; set; }
    }
}
