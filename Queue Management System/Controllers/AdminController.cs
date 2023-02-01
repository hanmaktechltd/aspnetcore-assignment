using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Contracts;
using Queue_Management_System.Models;

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

        // GET: Admin/ViewServiceProviders
        public async Task<ActionResult<IEnumerable<ServiceProviderVM>>> ViewServiceProviders()
        {
            var allGames = await _adminRepository.GetServiceProviders();
            return View(allGames);
        }

        // GET: Admin/ViewServiceProviderDetails/5
        public async Task<ActionResult<ServiceProviderVM>> ViewServiceProviderDetails(int id)
        {
            var ServiceProviderDetails = await _adminRepository.GetServiceProviderDetails(id);
            if (ServiceProviderDetails != null)
                return View(ServiceProviderDetails);
            else
                return NotFound();
        }

        // GET: Admin/CreateServiceProvider
        public ActionResult CreateServiceProvider()
        {
            return View();
        }

        // POST: Admin/CreateServiceProvider
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateServiceProvider(ServiceProviderVM serviceProvider)
        {
            await _adminRepository.CreateServiceProvider(serviceProvider);
            return RedirectToAction(nameof(ViewServiceProviders));
        }

        // GET: Admin/EditServiceProvider/5
        public async Task<ActionResult> EditServiceProvider(int id)
        {
            var serviceProvider = await _adminRepository.GetServiceProviderDetails(id);
            if (serviceProvider != null)
                return View(serviceProvider);
            else
                return NotFound();
        }

        // POST: Admin/EditServiceProvider/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditServiceProvider(int id, ServiceProviderVM serviceProvider)
        {
            await _adminRepository.UpdateServiceProvider(id, serviceProvider);
            return RedirectToAction(nameof(ViewServiceProviderDetails), new { id = serviceProvider.Id });
        }


        //TODO
        //Delete EditServiceProvider


        // GET: Admin/ViewServicePoints
        public async Task<ActionResult<IEnumerable<ServicePointVM>>> ViewServicePoints()
        {
            var servicePoints = await _adminRepository.GetServicePoints(); 
            return View(servicePoints);
        }
        // GET: Admin/ViewServicePointDetails/5
        public async Task<ActionResult<ServiceProviderVM>> ViewServicePointDetails(int id)
        {
            var ServiceProviderDetails = await _adminRepository.GetServicePointDetails(id);
            if (ServiceProviderDetails != null)
                return View(ServiceProviderDetails);
            else
                return NotFound();
        }









        // GET: Admin/CreateServicePoint
        public ActionResult CreateServicePoint()
        {
            return View();
        }

        // POST: Admin/CreateServicePoint
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateServicePoint(ServicePointVM servicePoint)
        {
            await _adminRepository.CreateServicePoint(servicePoint);
            return RedirectToAction(nameof(ViewServicePoints));
        }






    }
}
