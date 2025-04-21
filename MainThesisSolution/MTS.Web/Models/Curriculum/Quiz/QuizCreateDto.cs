using MTS.Web.Utility;
using System.ComponentModel.DataAnnotations;

namespace MTS.Web.Models.Curriculum.Quiz
{
    //USED
    public class QuizCreateDto
    {
        public string CourseCode { get; set; }
        public string WeekCode { get; set; }

        [Required]
        [Display(Name = "Quiz Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        [DateGreaterThan("StartTime", ErrorMessage = "End time must be after start time")]
        public DateTime EndTime { get; set; }

        [Required]
        [Range(1, 180, ErrorMessage = "Time limit must be between 1 and 180 minutes")]
        [Display(Name = "Time Limit (minutes)")]
        [TimeLimit("StartTime", "EndTime")]
        public int TimeLimit { get; set; }

        [Required]
        [Display(Name = "Quiz Type")]
        public string QuizType { get; set; } 
    }
}
