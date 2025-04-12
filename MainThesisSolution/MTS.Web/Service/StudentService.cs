using MTS.Web.Models;
using MTS.Web.Models.User.Student;
using MTS.Web.Service.IService;
using MTS.Web.Utility;

namespace MTS.Web.Service
{
    public class StudentService : IStudentService
    {
        private readonly IBaseService _baseService;

        public StudentService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> GetAllStudentsAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.UserAPIBase + "/api/students"
            });
        }

        public async Task<ResponseDto?> GetStudentByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.UserAPIBase}/api/students/{id}"
            });
        }

        public async Task<ResponseDto?> CreateStudentAsync(StudentCreateDto studentDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = studentDto,
                Url = SD.UserAPIBase + "/api/students"
            });
        }

        public async Task<ResponseDto?> UpdateStudentAsync(int id, StudentCreateDto studentDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = studentDto,
                Url = $"{SD.UserAPIBase}/api/students/{id}"
            });
        }

        public async Task<ResponseDto?> DeleteStudentAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = $"{SD.UserAPIBase}/api/students/{id}"
            });
        }
    }
}