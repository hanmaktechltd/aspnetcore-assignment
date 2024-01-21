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
                Services = services
            };
            return View(servicesViewModel);
        }

        public IActionResult ConfigureServiceProviders()
        {
            var serviceProviders = _serviceProviderRepository.GetServiceProviders();
            var serviceProvidersViewModel = new ServiceProvidersViewModel(){
                ServiceProviders = serviceProviders
            };
            return View(serviceProvidersViewModel);
        }

        public IActionResult ConfigureServicePoints()
        {
            var servicePoints = _servicePointRepository.GetServicePoints();
            var servicePointsViewModel = new ServicePointsViewModel(){
                ServicePoints = servicePoints
            };
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
        public IActionResult AddService(ServiceModel service)
        {
            if (ModelState.IsValid)
            {
                _serviceRepository.AddService(service);
                return RedirectToAction("ConfigureServices");
            }
            return View();
        }

        [HttpGet]
        public IActionResult EditService(string id)
        {
            var service = _serviceRepository.GetServiceById(id);
            return View(service);
        }

        [HttpPost]
        public IActionResult EditService(ServiceModel service)
        {
            if (ModelState.IsValid)
            {
                _serviceRepository.UpdateService(service);
                return RedirectToAction("ConfigureServices");
            }
            return View();
        }

        public IActionResult DeleteService(string id)
        {
            _serviceRepository.DeleteService(id);
            return RedirectToAction("ConfigureServices");
        }

        [HttpGet]
        public IActionResult AddServiceProvider()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddServiceProvider(ServiceProviderModel serviceProvider)
        {
            if (ModelState.IsValid)
            {
                _serviceProviderRepository.AddServiceProvider(serviceProvider);
                return RedirectToAction("ConfigureServiceProviders");
            }
            return View();
        }

        [HttpGet]
        public IActionResult EditServiceProvider(string id)
        {
            var serviceProvider = _serviceProviderRepository.GetServiceProviderById(id);
            return View(serviceProvider);
        }

        [HttpPost]
        public IActionResult EditServiceProvider(ServiceProviderModel serviceProvider)
        {
            if (ModelState.IsValid)
            {
                _serviceProviderRepository.UpdateServiceProvider(serviceProvider);
                return RedirectToAction("ConfigureServiceProviders");
            }
            return View();
        }

        public IActionResult DeleteServiceProvider(string id)
        {
            _serviceProviderRepository.DeleteServiceProvider(id);
            return RedirectToAction("ConfigureServiceProviders");
        }

        [HttpGet]
        public IActionResult AddServicePoint()
        {
            var services = _serviceRepository.GetServices();
            var serviceProviders = _serviceProviderRepository.GetServiceProviders();
            ViewData["Services"] = services;
            ViewData["ServiceProviders"] = serviceProviders;
            return View();
        }

        [HttpPost]
        public IActionResult AddServicePoint(ServicePointModel servicePoint)
        {
            if (ModelState.IsValid)
            {
                _servicePointRepository.AddServicePoint(servicePoint);
                return RedirectToAction("ConfigureServicePoints");
            }
            var services = _serviceRepository.GetServices();
            var serviceProviders = _serviceProviderRepository.GetServiceProviders();
            ViewData["Services"] = services;
            ViewData["ServiceProviders"] = serviceProviders;
            return View();
        }

        [HttpGet]
        public IActionResult EditServicePoint(string id)
        {
            var servicePoint = _servicePointRepository.GetServicePointById(id);
            return View(servicePoint);
        }

        [HttpPost]
        public IActionResult EditServicePoint(ServicePointModel servicePoint)
        {
            if (ModelState.IsValid)
            {
                _servicePointRepository.UpdateServicePoint(servicePoint);
                return RedirectToAction("ConfigureServicePoints");
            }
            return View();
        }

        public IActionResult DeleteServicePoint(string id)
        {
            _servicePointRepository.DeleteServicePoint(id);
            return RedirectToAction("ConfigureServicePoints");
        }

    }
}
