using AutoMapper;
using CBS.Services.OrderAPI.Data;
using CBS.Services.OrderAPI.Models.DTO;
using CBS.Services.OrderAPI.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace CBS.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : Controller
    {
        protected ResponseDto _response;
        private IMapper _mapper;
        private readonly AppDbContext _db;
        private IProductService _productService;
        //private readonly IMessageBus2 _messageBus;
        private readonly IConfiguration _configuration;
        public IActionResult Index()
        {
            return View();
        }
    }
}
