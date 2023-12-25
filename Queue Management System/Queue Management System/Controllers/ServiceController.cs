using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;
using Queue_Management_System.Repository;
using System.Threading.Tasks;

namespace Queue_Management_System.Controllers
{
    public class ServiceController : Controller
    {
        private readonly DbOperationsRepository _dbOperationsRepository;

        public ServiceController(DbOperationsRepository dbOperationsRepository)
        {
            _dbOperationsRepository = dbOperationsRepository;
        }

        [HttpGet]
        public async Task<IActionResult> ServiceSelection()
        {
            var serviceTypes = await _dbOperationsRepository.GetAvailableServicesAsync();
            return View(serviceTypes);
        }

        [HttpPost]
        public async Task<IActionResult> SelectService(int selectedServiceId)
        {
            // Perform actions with the selected service ID
            // For example, store it in the database for the logged-in user

            // Redirect to a success page or perform further actions
            return RedirectToAction("Success", "Service");
        }

        // Add other controller actions as needed
    }
}
