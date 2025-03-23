using MTS.Web.Models;
using MTS.Web.Service.IService;
using MTS.Web.Utility;
using System;

namespace MTS.Web.Service
{
    public class AdminService : IAdminService
    {
        private readonly IBaseService _baseService;

        public AdminService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto?> GenerateIds(UniversityIdGenerateDto universityIdGenerateDto)
        {
            var something = SD.UserAPIBase;
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = universityIdGenerateDto,
                Url = SD.UserAPIBase + "/api/universityids/generate"
            });
        }

        public async Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registrationRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/AssignRole"
            });
        }

        public async Task<ResponseDto?> UpdateStudentAsync(StudentDto studentDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = studentDto,
                Url = SD.UserAPIBase + "/api/students/" + studentDto.ID
            });
        }
        public async Task<ResponseDto?> GetStudentsAsync()
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
                Url = SD.UserAPIBase + "/api/students/" + id
            });
        }
        public async Task<ResponseDto?> GetProfessorsAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.UserAPIBase + "api/professors"
            });
        }

        public Task<ResponseDto?> GetUnassignedIds(string type)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto?> VerifyId(UniversityIdVerifyDto universityIdGenerateDto)
        {
            throw new NotImplementedException();
        }
    }
}
