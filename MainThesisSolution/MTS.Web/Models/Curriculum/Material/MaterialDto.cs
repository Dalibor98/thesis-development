namespace MTS.Web.Models.Curriculum.Material
{
    public class MaterialDto
    {
        public int Id { get; set; }
        public string CourseCode { get; set; }
        public string WeekCode { get; set; }
        public string MaterialCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string MaterialType { get; set; } // Video, PDF, Link, etc.
    }
}
