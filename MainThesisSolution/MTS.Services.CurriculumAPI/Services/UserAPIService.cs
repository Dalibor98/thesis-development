using Newtonsoft.Json;
using MTS.Services.CurriculumAPI.Models.DTO;
using MTS.Services.CurriculumAPI.Services.IService;

namespace MTS.Services.CurriculumAPI.Services
{
    public class UserAPIService : IUserAPIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public UserAPIService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ServiceUrls:UserAPIBase"]);
        }

        public async Task<bool> ProfessorExistsAsync(string professorUniversityId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/professors/verify/{professorUniversityId}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ResponseDto>(jsonString);

                    if (result.IsSuccess)
                    {
                        return Convert.ToBoolean(result.Result);
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                //Should I log the exception, how does this propagate further? ? 
                return false;
            }
        }
    }
}