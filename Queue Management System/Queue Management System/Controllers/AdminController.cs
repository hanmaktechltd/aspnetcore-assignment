using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;
using Queue_Management_System.Repositories;

namespace Queue_Management_System.Controllers
{
    [Authorize(Roles = "Admin")]
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

        public async Task<IActionResult> ConfigureServices()
        {
            var services = await _serviceRepository.GetServices();
            var servicesViewModel = new ServicesViewModel(){
                Services = services
            };
            
            return View(servicesViewModel);
        }

        public async Task<IActionResult> ConfigureServiceProviders()
        {
            var serviceProviders = await _serviceProviderRepository.GetServiceProviders();
            var serviceProvidersViewModel = new ServiceProvidersViewModel(){
                ServiceProviders = serviceProviders
            };
            return View(serviceProvidersViewModel);
        }

        public async Task<IActionResult> ConfigureServicePoints()
        {
            var servicePoints = await _servicePointRepository.GetServicePoints();
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
        public async Task<IActionResult> AddService(ServiceModel service)
        {
            if (ModelState.IsValid)
            {
                await _serviceRepository.AddService(service);
                return RedirectToAction("ConfigureServices");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditService(string id)
        {
            var service = await _serviceRepository.GetServiceById(id);
            return View(service);
        }

        [HttpPost]
        public async Task<IActionResult> EditService(ServiceModel service)
        {
            if (ModelState.IsValid)
            {
                await _serviceRepository.UpdateService(service);
                return RedirectToAction("ConfigureServices");
            }
            return View();
        }

        public async Task<IActionResult> DeleteService(string id)
        {
            await _serviceRepository.DeleteService(id);
            return RedirectToAction("ConfigureServices");
        }

        [HttpGet]
        public IActionResult AddServiceProvider()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddServiceProvider(ServiceProviderModel serviceProvider)
        {
            if (ModelState.IsValid)
            {
                await _serviceProviderRepository.AddServiceProvider(serviceProvider);
                return RedirectToAction("ConfigureServiceProviders");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditServiceProvider(string id)
        {
            var serviceProvider = await _serviceProviderRepository.GetServiceProviderById(id);
            return View(serviceProvider);
        }

        [HttpPost]
        public async Task<IActionResult> EditServiceProvider(ServiceProviderModel serviceProvider)
        {
            if (ModelState.IsValid)
            {
                await _serviceProviderRepository.UpdateServiceProvider(serviceProvider);
                return RedirectToAction("ConfigureServiceProviders");
            }
            return View();
        }

        public async Task<IActionResult> DeleteServiceProvider(string id)
        {
            await _serviceProviderRepository.DeleteServiceProvider(id);
            return RedirectToAction("ConfigureServiceProviders");
        }

        [HttpGet]
        public async Task<IActionResult> AddServicePoint()
        {
            var services = await _serviceRepository.GetServices();
            var serviceProviders = await _serviceProviderRepository.GetServiceProviders();
            ViewData["Services"] = services;
            ViewData["ServiceProviders"] = serviceProviders;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddServicePoint(ServicePointModel servicePoint)
        {
            if (ModelState.IsValid)
            {
                await _servicePointRepository.AddServicePoint(servicePoint);
                return RedirectToAction("ConfigureServicePoints");
            }
            var services = await _serviceRepository.GetServices();
            var serviceProviders = await _serviceProviderRepository.GetServiceProviders();
            ViewData["Services"] = services;
            ViewData["ServiceProviders"] = serviceProviders;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditServicePoint(string id)
        {
            var services = await _serviceRepository.GetServices();
            var serviceProviders = await _serviceProviderRepository.GetServiceProviders();
            var servicePoint = await _servicePointRepository.GetServicePointById(id);
            ViewData["Services"] = services;
            ViewData["ServiceProviders"] = serviceProviders;
            
            return View(servicePoint);
        }

        [HttpPost]
        public async Task<IActionResult> EditServicePoint(ServicePointModel servicePoint)
        {
            if (ModelState.IsValid)
            {
                await _servicePointRepository.UpdateServicePoint(servicePoint);
                return RedirectToAction("ConfigureServicePoints");
            }
            return View();
        }

        public async Task<IActionResult> DeleteServicePoint(string id)
        {
            await _servicePointRepository.DeleteServicePoint(id);
            return RedirectToAction("ConfigureServicePoints");
        }

    }
}
