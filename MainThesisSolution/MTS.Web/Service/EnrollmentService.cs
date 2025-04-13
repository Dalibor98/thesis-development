using MTS.Web.Models;
using MTS.Web.Models.Curriculum.Course;
using MTS.Web.Service.IService;
using MTS.Web.Utility;

namespace MTS.Web.Service
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IBaseService _baseService;

        public EnrollmentService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> GetStudentEnrollmentsAsync(string studentUniversityId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/enrollments/student/{studentUniversityId}"
            });
        }

        public async Task<ResponseDto?> GetCourseEnrollmentsAsync(string courseCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/enrollments/course/{courseCode}"
            });
        }

        public async Task<ResponseDto?> EnrollStudentAsync(string courseCode, string studentUniversityId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = new { CourseCode = courseCode, StudentCode = studentUniversityId },
                Url = SD.CurriculumAPIBase + "/api/enrollments"
            });
        }

        public async Task<ResponseDto?> DropCourseAsync(int enrollmentId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = new { Id = enrollmentId, RegistrationStatus = "Dropped" },
                Url = SD.CurriculumAPIBase + "/api/enrollments"
            });
        }

        public async Task<ResponseDto?> IsStudentEnrolledAsync(string courseCode, string studentUniversityId)
        {
            var temp = SD.CurriculumAPIBase + $"/api/enrollments/check/{courseCode}/{studentUniversityId}";
            string encodedCourseCode = Uri.EscapeDataString(courseCode);
            string encodedStudentId = Uri.EscapeDataString(studentUniversityId);
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/enrollments/check/{encodedCourseCode}/{encodedStudentId}"
            });
        }
    }
}