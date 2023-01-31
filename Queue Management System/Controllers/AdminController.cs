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
        public async Task<ActionResult<IEnumerable<AdminVM>>> ViewServiceProviders()
        {
            var allGames = await _adminRepository.GetAll();//rename this to  GetServiceProviders
            return View(allGames);
        }
       

       

    }
}
