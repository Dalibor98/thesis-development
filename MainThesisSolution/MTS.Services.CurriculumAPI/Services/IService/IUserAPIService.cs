namespace MTS.Services.CurriculumAPI.Services.IService
{
    public interface IUserAPIService
    {
        Task<bool> ProfessorExistsAsync(string professorUniversityId);

        //Other user-related methods as needed
    }
}