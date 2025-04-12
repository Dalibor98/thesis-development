using Microsoft.VisualBasic;
using MTS.Web.Utility;
using System.ComponentModel.DataAnnotations;

namespace MTS.Web.Models.User.Student
{
    public class StudentCreateDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 100 characters")]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        public string UniversityId { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [CustomPassword]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Major is required")]
        [Display(Name = "Academic Major")]
        public string Major { get; set; }

        [Required(ErrorMessage = "Enrollment Year is required")]
        [Range(2000, 2050, ErrorMessage = "Enrollment Year must be between 2000 and 2050")]
        [Display(Name = "Enrollment Year")]
        public int EnrollmentYear { get; set; }
    }
}