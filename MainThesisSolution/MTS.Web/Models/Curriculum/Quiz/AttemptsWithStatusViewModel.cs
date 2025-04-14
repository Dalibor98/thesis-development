namespace MTS.Web.Models.Curriculum.Quiz
{
    public class AttemptWithStatusViewModel
    {
        public StudentQuizAttemptDto Attempt { get; set; }
        public bool NeedsGrading { get; set; }
    }
}
