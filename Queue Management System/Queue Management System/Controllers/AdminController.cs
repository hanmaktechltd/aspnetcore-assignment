using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;
using Queue_Management_System.Repositories;

namespace Queue_Management_System.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IServiceRepository _serviceRepository;
        
        private readonly IServiceProviderRepository _serviceProviderRepository;

        private readonly IServicePointRepository _servicePointRepository;

        public AdminController(IServiceRepository serviceRepository, IServiceProviderRepository serviceProviderRepository, IServicePointRepository servicePointRepository)
        {
            _serviceRepository = serviceRepository;
            _serviceProviderRepository = serviceProviderRepository;
            _servicePointRepository = servicePointRepository;
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult ConfigureServices()
        {
            var services = _serviceRepository.GetServices();
            var servicesViewModel = new ServicesViewModel(){
                Services = services;
            }
            return View(servicesViewModel);
        }

        public IActionResult ConfigureServiceProviders()
        {
            var serviceProviders = _serviceProviderRepository.GetServiceProviders();
            var serviceProvidersViewModel = new ServiceProvidersViewModel(){
                ServiceProviders = serviceProviders;
            }
            return View(serviceProvidersViewModel);
        }

        public IActionResult ConfigureServicePoints()
        {
            var servicePoints = _servicePointRepository.GetServicePoints();
            var servicePointsViewModel = new ServicePointsViewModel(){
                ServicePoints = servicePoints;
            }
            return View(servicePointsViewModel);
        }

        public IActionResult GenerateAnalyticalReport()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddService()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddService()
        {
            return View();
        }

        [HttpGet]
        public IActionResult EditService()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EditService()
        {
            return View();
        }

        public IActionResult DeleteService()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddServiceProvider()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddServiceProvider()
        {
            return View();
        }

        [HttpGet]
        public IActionResult EditServiceProvider()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EditServiceProvider()
        {
            return View();
        }

        public IActionResult DeleteServiceProvider()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddServicePoint()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddServicePoint()
        {
            return View();
        }

        [HttpGet]
        public IActionResult EditServicePoint()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EditServicePoint()
        {
            return View();
        }

        public IActionResult DeleteServicePoint()
        {
            return View();
        }

    }
}
