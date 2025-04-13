using MTS.Web.Models;
using MTS.Web.Models.Curriculum.Course;
using MTS.Web.Service.IService;
using MTS.Web.Utility;
using System;

namespace MTS.Web.Service
{
    public class CourseService : ICourseService
    {
        private readonly IBaseService _baseService;

        public CourseService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> GetAllCoursesAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + "/api/courses"
            });
        }

        public async Task<ResponseDto?> GetCourseByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.CurriculumAPIBase}/api/courses/{id}"
            });
        }

        public async Task<ResponseDto?> GetCourseByCodeAsync(string courseCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.CurriculumAPIBase}/api/courses/code/{courseCode}"
            });
        }

        public async Task<ResponseDto?> CreateCourseAsync(TemporaryCourseDTO courseDto)
        {
            var temp = SD.CurriculumAPIBase + "/api/courses";

            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = courseDto,
                Url = SD.CurriculumAPIBase + "/api/courses"
            });
        }

        public async Task<ResponseDto?> UpdateCourseAsync(CourseUpdateDto courseDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = courseDto,
                Url = SD.CurriculumAPIBase + "/api/courses"
            });
        }

        public async Task<ResponseDto?> DeleteCourseAsync(int id)
        {
            var temp = $"{SD.CurriculumAPIBase}/api/courses/{id}";
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = $"{SD.CurriculumAPIBase}/api/courses/{id}"
            });
        }

        public async Task<ResponseDto?> GetWeeksByCourseCodeAsync(string courseCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.CurriculumAPIBase}/api/courses/{courseCode}/weeks"
            });
        }

        public async Task<ResponseDto?> GetMaterialsByCourseCodeAsync(string courseCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.CurriculumAPIBase}/api/courses/{courseCode}/materials"
            });
        }

        public async Task<ResponseDto?> GetAssignmentsByCourseCodeAsync(string courseCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.CurriculumAPIBase}/api/courses/{courseCode}/assignments"
            });
        }

        public async Task<ResponseDto?> GetProfessorCoursesAsync(string professorId)
        {
            var temp = $"{SD.CurriculumAPIBase}/api/courses/professor/{professorId}";
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.CurriculumAPIBase}/api/courses/professor/{professorId}"
            });
        }
        public async Task<ResponseDto?> GetQuizzesByCourseCodeAsync(string courseCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.CurriculumAPIBase}/api/courses/{courseCode}/quizzes"
            });
        }
        public async Task<ResponseDto?> GetStudentCoursesAsync(string studentId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.CurriculumAPIBase}/api/courses/student/{studentId}"
            });
        }

        public async Task<ResponseDto?> GetRegistrationsByCourseCodeAsync(string courseCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.CurriculumAPIBase}/api/courses/{courseCode}/registrations"
            });
        }
    }
}