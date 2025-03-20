using System.ComponentModel.DataAnnotations;

namespace CBS.Services.PlumAPI.Models
{
    public class Plum
    {
        [Key]
        public int PlumId { get; set; }
        [Required]
        public string Name { get; set; }
        [Range(1, 1000)]
        public double Price { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
    }
}
