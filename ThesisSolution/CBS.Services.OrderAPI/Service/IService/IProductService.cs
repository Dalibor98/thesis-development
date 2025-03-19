using CBS.Services.OrderAPI.Models.DTO;

namespace CBS.Services.OrderAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
