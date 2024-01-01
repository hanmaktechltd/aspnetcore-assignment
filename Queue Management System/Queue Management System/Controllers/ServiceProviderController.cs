using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Queue_Management_System.Controllers
{
    public class ServiceProviderController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly IServicePointService _servicePointService;
        private readonly IServiceProviderService _serviceProviderService;
        private readonly ILogger<QueueController> _logger;

        public ServiceProviderController(ITicketService ticketService, IServicePointService servicePointService, IServiceProviderService serviceProviderService, ILogger<QueueController> logger)
        {
            _ticketService = ticketService;
            _servicePointService = servicePointService;
            _serviceProviderService = serviceProviderService;
            _logger = logger;
        }

        public IActionResult Create()
        {
            var allServicePoints = _servicePointService.GetServicePoints();

            var viewModel = new ServiceProviderViewModel
            {
                Roles = GetRoleSelectList(),
                AllServicePoints = GetServicePointSelectList(allServicePoints)
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Create(ServiceProviderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var serviceProvider = new ServiceProvider
                {
                    Username = model.Username,
                    Role = model.Role,
                };

                _serviceProviderService.AddServiceProviderWithServicePoints(serviceProvider, model.SelectedServicePointIds);
                return RedirectToAction("Dashboard", "Admin");
            }

             var allServicePoints = _servicePointService.GetServicePoints();

            var newViewModel = new ServiceProviderViewModel
            {
                Roles = GetRoleSelectList(),
                AllServicePoints = GetServicePointSelectList(allServicePoints)
            };


            LogModelStateErrors();
            return View(newViewModel);
        }

        public IActionResult Edit(int id)
        {
            ServiceProvider serviceProvider = _serviceProviderService.GetServiceProviderWithServicePointsById(id);
            var allServicePoints = _servicePointService.GetServicePoints();
            var selectedServicePointIds = serviceProvider.ServicePoints.Select(sp => sp.ServicePointId).ToList();

            var editViewModel = new ServiceProviderViewModel
            {
                ServiceProviderId = serviceProvider.ServiceProviderId,
                Username = serviceProvider.Username,
                Role = serviceProvider.Role,
                Roles = GetRoleSelectList(),
                AllServicePoints = GetServicePointSelectList(allServicePoints, selectedServicePointIds),
                SelectedServicePointIds = selectedServicePointIds
            };

            return View(editViewModel);
        }

        [HttpPost]
        public IActionResult Edit(ServiceProviderViewModel model)
        {
            _logger.LogInformation($"Editing ServiceProvider - ServiceProviderId: {model.ServiceProviderId}, Username: {model.Username}, Role: {model.Role}");

            if (ModelState.IsValid)
            {
                var serviceProvider = new ServiceProvider
                {
                    ServiceProviderId = model.ServiceProviderId,
                    Username = model.Username,
                    Role = model.Role
                };

                _serviceProviderService.UpdateServiceProviderWithServicePoints(serviceProvider, model.SelectedServicePointIds);
                return RedirectToAction("Dashboard", "Admin");
            }

            var allServicePoints = _servicePointService.GetServicePoints();

            var newViewModel = new ServiceProviderViewModel
            {
                Roles = GetRoleSelectList(),
                AllServicePoints = GetServicePointSelectList(allServicePoints)
            };

            return View(newViewModel);
        }

        public IActionResult Delete(int id)
        {
            ServiceProvider serviceProvider = _serviceProviderService.GetServiceProviderById(id);

            if (serviceProvider == null)
            {
                return NotFound();
            }

            return View(serviceProvider);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            _serviceProviderService.DeleteServiceProviderWithServicePoints(id);
            return RedirectToAction("Dashboard", "Admin");
        }

        private void LogModelStateErrors()
        {
            foreach (var entry in ModelState)
            {
                foreach (var error in entry.Value.Errors)
                {
                    Console.WriteLine($"Property: {entry.Key}, Error: {error.ErrorMessage}");
                }
            }
        }

        private List<SelectListItem> GetRoleSelectList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "super", Text = "Super" },
                new SelectListItem { Value = "regular", Text = "Regular" }
            };
        }

        private List<SelectListItem> GetServicePointSelectList(List<ServicePoint> servicePoints, List<int> selectedServicePointIds = null)
        {
            return servicePoints
                .Select(sp => new SelectListItem
                {
                    Value = sp.ServicePointId.ToString(),
                    Text = sp.ServicePointName,
                    Selected = selectedServicePointIds?.Contains(sp.ServicePointId) ?? false
                })
                .ToList();
        }
    }
}
