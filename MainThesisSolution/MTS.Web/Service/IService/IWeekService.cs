using MTS.Web.Models;
using MTS.Web.Models.Curriculum.Week;

namespace MTS.Web.Service.IService
{
    public interface IWeekService
    {
        Task<ResponseDto?> GetWeekByIdAsync(int id);
        Task<ResponseDto?> GetWeekByCodeAsync(string weekCode);
        Task<ResponseDto?> GetWeeksByCourseCodeAsync(string courseCode);
        Task<ResponseDto?> CreateWeekAsync(WeekCreateDto weekDto);
        Task<ResponseDto?> UpdateWeekAsync(WeekUpdateDto weekUpdateDto);
        Task<ResponseDto?> DeleteWeekAsync(int id);
        Task<ResponseDto?> DeleteWeekByCodeAsync(string weekCode);
        Task<ResponseDto?> GetMaterialsByWeekCodeAsync(string weekCode);
        Task<ResponseDto?> GetAssignmentsByWeekCodeAsync(string weekCode);
        Task<ResponseDto?> GetQuizzesByWeekCodeAsync(string weekCode);
    }
}
