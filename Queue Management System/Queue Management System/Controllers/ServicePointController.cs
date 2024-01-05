using Microsoft.AspNetCore.Mvc;

namespace Queue_Management_System.Controllers
{
    public class ServicePointController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly IServicePointService _servicePointService;
        private readonly ILogger<ServicePointController> _logger;

        public ServicePointController(ITicketService ticketService, IServicePointService servicePointService, ILogger<ServicePointController> logger)
        {
            _ticketService = ticketService;
            _servicePointService = servicePointService;
            _logger = logger;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ServicePoint model)
        {
            if (ModelState.IsValid)
            {
                _servicePointService.CreateServicePoint(model);
                return RedirectToAction("Dashboard", "Admin");
            }

            LogModelStateErrors();
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            ServicePoint servicePoint = _servicePointService.GetServicePointById(id);
            return View(servicePoint);
        }

        [HttpPost]
        public IActionResult Edit(ServicePoint model)
        {
            if (ModelState.IsValid)
            {
                _servicePointService.UpdateServicePoint(model);
                _logger.LogInformation("ServicePoint updated successfully: {ServicePointId}", model.ServicePointId);
                return RedirectToAction("Dashboard", "Admin");
            }

            return View(model);
        }

        public IActionResult Delete(int id)
        {
            var servicePoint = _servicePointService.GetServicePointById(id);
            return View(servicePoint);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            _servicePointService.DeleteServicePoint(id);
            return RedirectToAction("Dashboard", "Admin");
        }

        private void LogModelStateErrors()
        {
            foreach (var entry in ModelState)
            {
                foreach (var error in entry.Value.Errors)
                {
                    _logger.LogError($"Property: {entry.Key}, Error: {error.ErrorMessage}");
                }
            }
        }
    }
}

