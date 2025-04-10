namespace MTS.Services.CurriculumAPI.Models.DTO
{
    public class MaterialUpdateDto
    {
        public string CourseCode { get; set; }
        public string WeekCode { get; set; }
        public string MaterialCode { get; set; }
        public string Title { get; set; }

        //public string FileUrl { get; set; }
        public string Description { get; set; }
        public string MaterialType { get; set; }
    }
}
