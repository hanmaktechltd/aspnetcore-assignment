using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Queue_Management_System.Models;
using Queue_Management_System.Services;
using System.Net;

namespace Queue_Management_System.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IAdminRepository _adminRepository;
        public AdminController(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

        // GET: Admin/ServicePoints
        public async Task<ActionResult> ServicePoints()
        {
            var servicePoints = await _adminRepository.GetServicePoints();
            if (servicePoints != null)
                return View(servicePoints);
            return NotFound();
        }

        // GET: Admin/CreateServicePoint
        public async Task<ActionResult> CreateServicePoint()
        {
            return View();
        }

        // POST: Admin/CreateServicePoint
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateServicePoint(ServicePointM servicePoint)
        {
            await _adminRepository.CreateServicePoint(servicePoint);
            return RedirectToAction(nameof(ServicePoints));
        }
        // GET: Admin/EditServicePoint/5
        public async Task<ActionResult> EditServicePoint(int id)
        {
            ServicePointM servicePoint = await _adminRepository.GetServicePointDetails(id);
            if (servicePoint != null)
                return View(servicePoint);
            return NotFound();
        }
        // POST: Admin/EditServicePoint/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditServicePoint(ServicePointM servicePoint)
        {
            await _adminRepository.UpdateServicePoint( servicePoint);
            return RedirectToAction(nameof(ViewServicePointDetails), new { id = servicePoint.Id });
        }


        // GET: Admin/ViewServicePointDetails/5
        public async Task<ActionResult> ViewServicePointDetails(int id)
        {
            ServicePointM ServiceProviderDetails = await _adminRepository.GetServicePointDetails(id);
            if (ServiceProviderDetails != null)
                return View(ServiceProviderDetails);
            return NotFound();
        }

        // POST: Admin/DeleteServicePoint/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteServicePoint(int id)
        {
            await _adminRepository.DeleteServicePoint(id);
            return RedirectToAction(nameof(ServicePoints));
        }
        public IActionResult Reports()
        {
            return View();
        }


        //service providers

        // GET: Admin/ViewServiceProviders
        public async Task<ActionResult> ServiceProviders()
        {
            var serviceProviders = await _adminRepository.GetServiceProviders();
            return View(serviceProviders);
        }

        // GET: Admin/ViewServiceProviderDetails/5
        public async Task<ActionResult> ServiceProviderDetails(int id)
        {
            ServiceProviderM serviceProviderDetails = await _adminRepository.GetServiceProviderDetails(id);
            if (serviceProviderDetails != null)
                return View(serviceProviderDetails);
            return NotFound();
        }

        // GET: Admin/CreateServiceProvider
        public async Task<ActionResult> CreateServiceProvider()
        {
            return View();
        }

        // POST: Admin/CreateServiceProvider
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateServiceProvider(ServiceProviderM serviceProvider)
        {
            await _adminRepository.CreateServiceProvider(serviceProvider);
            return RedirectToAction(nameof(ServiceProviders));
        }

        // POST: Admin/EditServiceProvider/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditServiceProvider(ServiceProviderM serviceProvider)
        {
            await _adminRepository.UpdateServiceProvider(serviceProvider);
            return RedirectToAction(nameof(ServiceProviderDetails), new { id = serviceProvider.Id });
        }
        // POST: Admin/DeleteServiceProvider/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteServiceProvider(int id)
        {
            await _adminRepository.DeleteServiceProvider(id);
            return RedirectToAction(nameof(ServiceProviders));
        }

    }
}
