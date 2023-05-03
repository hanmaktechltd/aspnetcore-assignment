using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.Extensions.Hosting.Internal;
using Queue_Management_System.Services;

namespace Queue_Management_System.Controllers
{
    public class CheckInController : Controller
    {
        private readonly ICheckInRepository _checkInRepository;
        private readonly IAdminRepository _adminRepository;

        public CheckInController(ICheckInRepository checkInRepository, IAdminRepository adminRepository)
        {
            _checkInRepository = checkInRepository;
            _adminRepository = adminRepository;
        }

        public async Task<IActionResult> Index()
        {
            var servicePoints = await _adminRepository.GetAllServicePoints();
            return View(servicePoints);
        }

        [HttpPost]
        public async Task<IActionResult> CheckInAsync(int servicePointId, int serviceProviderId)
        {

            TempData["success"] = "Ticket Generated successfully";
            return await _checkInRepository.CheckIn(servicePointId, serviceProviderId);
        }

    }
}
