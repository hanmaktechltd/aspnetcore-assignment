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
            
            return RedirectToAction("Success", "Service");
        }

   }
}
