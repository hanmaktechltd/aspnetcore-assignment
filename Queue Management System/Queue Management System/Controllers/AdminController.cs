using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Queue_Management_System.Controllers
{
    [Authorize(Policy = "RequireAdminRole")]
    public class AdminController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly IServicePointService _servicePointService;

        private readonly IServiceProviderService _serviceProviderService;
        private readonly ILogger<QueueController> _logger;

        public AdminController(ITicketService ticketService, IServicePointService servicePointService, IServiceProviderService serviceProviderService, ILogger<QueueController> logger)
        {
            _ticketService = ticketService;
            _servicePointService = servicePointService;
            _serviceProviderService = serviceProviderService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            var viewModel = new DashboardViewModel
            {
                ServicePoints = _servicePointService.GetServicePoints(),
                ServiceProviders = _serviceProviderService.GetServiceProvidersWithServicePoints()
    
            };    

            return View(viewModel);
        }
    }
}
