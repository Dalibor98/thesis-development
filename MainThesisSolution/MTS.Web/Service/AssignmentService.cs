using MTS.Web.Models;
using MTS.Web.Service.IService;
using MTS.Web.Utility;

namespace MTS.Web.Service
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IBaseService _baseService;

        public AssignmentService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> GetAssignmentByCodeAsync(string assignmentCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/assignments/code/{assignmentCode}"
            });
        }

        public async Task<ResponseDto?> GetSubmissionsByAssignmentCodeAsync(string assignmentCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/assignments/{assignmentCode}/submissions"
            });
        }

        public async Task<ResponseDto?> GetStudentSubmissionAsync(string assignmentCode, string studentUniversityId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CurriculumAPIBase + $"/api/assignmentAttempts/student/{studentUniversityId}/assignment/{assignmentCode}"
            });
        }

        public async Task<ResponseDto?> SubmitAssignmentAsync(StudentAssignmentAttemptCreateDto submissionDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = submissionDto,
                Url = SD.CurriculumAPIBase + "/api/assignmentAttempts"
            });
        }

        public async Task<ResponseDto?> GradeAssignmentAsync(StudentAssignmentGradeDto gradeDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = gradeDto,
                Url = SD.CurriculumAPIBase + $"/api/assignmentAttempts/{gradeDto.SubmissionId}"
            });
        }
    }
}