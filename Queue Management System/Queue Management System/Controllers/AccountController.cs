using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;
using Queue_Management_System.Repository;
using Queue_Management_System.ServiceInterface;
using System.Threading.Tasks;

namespace Queue_Management_System.Controllers
{
    public class AccountController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly DbOperationsRepository _dbOperationsRepository;
        private static ServiceProviderModel loggedInUser;

        public AccountController(ITicketService ticketService, DbOperationsRepository dbOperationsRepository)
        {
            _ticketService = ticketService;
            _dbOperationsRepository = dbOperationsRepository;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _dbOperationsRepository.LoginAsync(model.UsernameOrEmail, model.Password);
                if (user != null)
                {
                    
                    // Authentication successful, store user data
                    loggedInUser = user;

                    return RedirectToAction("ServiceSelection");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt");
                    return View(model);
                }
            }

            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> ServiceSelection()
        {
            var serviceTypes = await _ticketService.GetAvailableServicesAsync();
            return View(serviceTypes);
        }

        [HttpGet]
        public async Task<IActionResult> SelectService()
        {
            var services = await _ticketService.GetAvailableServicesAsync();

            if (services != null)
            {
                return View(services);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SelectService(int selectedServiceId)
        {

            if (loggedInUser != null)
            {
                if (loggedInUser.ServiceTypeId == selectedServiceId)
                {
                    // Perform actions when the service types match
                    return RedirectToAction("ServicePoint", "Queue");

                }
                else
                {
                    // Perform actions when the service types do not match
                    // Redirect to an error page or perform different logic
                    return RedirectToAction("ServiceSelection");
                }
            }
            else
            {
                // Handle case where login fails or serviceProvider is null
                return RedirectToAction("Login", "Account"); // Redirect to login page or handle appropriately
            }
        }
        public IActionResult GetLoggedInUser()
        {
            // Use 'loggedInUser' as needed, such as displaying user information
            if (loggedInUser != null)
            {
                // Return the user data or perform actions with loggedInUser
                return Json(loggedInUser);
            }
            else
            {
                return Json("No user logged in");
            }
        }
    }
}
