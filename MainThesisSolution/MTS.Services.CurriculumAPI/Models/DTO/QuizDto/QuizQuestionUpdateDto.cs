using System.ComponentModel.DataAnnotations;

namespace MTS.Services.CurriculumAPI.Models.DTO.QuizDto
{
    public class QuizQuestionUpdateDto
    { 
        public string QuizCode { get; set; }      
        public string QuizQuestionCode { get; set; }      
        public string QuestionText { get; set; }       
        public int Points { get; set; }
    }
}
