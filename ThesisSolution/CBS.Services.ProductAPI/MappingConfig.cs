using AutoMapper;
using CBS.Services.ProductAPI.Models;
using CBS.Services.ProductAPI.Models.DTO;

namespace CBS.Services.ProductAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductDto, Product>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
