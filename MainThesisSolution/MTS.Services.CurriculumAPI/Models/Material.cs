namespace MTS.Services.CurriculumAPI.Models
{
    public class Material
    {
        public int Id { get; set; }
        public string CourseCode { get; set; }
        public string WeekCode { get; set; }
        public string MaterialCode { get; set; }
        public string Title { get; set; }
        //public string FileUrl { get; set; }
        public string Description { get; set; }
        public string MaterialType { get; set; } // Video, PDF, Link, etc.
        /*
         public static string GenerateMaterialCode(string weekCode)
        {
            return $"{weekCode}-MAT-{Guid.NewGuid().ToString().Substring(0, 6)}";
        }*/
    }
}
