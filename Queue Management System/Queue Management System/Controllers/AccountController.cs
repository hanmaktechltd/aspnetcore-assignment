using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;
using Queue_Management_System.Repository;
using Queue_Management_System.ServiceInterface;
using Queue_Management_System.Services;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Queue_Management_System.Controllers
{
    public class AccountController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly DbOperationsRepository _dbOperationsRepository;
        private static ServiceProviderModel loggedInUser;
        private readonly string issuer = "Admin";
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration, ITicketService ticketService, DbOperationsRepository dbOperationsRepository)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration)); // Assign configuration to _configuration
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
                    //loggedInUser = user;
                    UserUtility.SetLoggedInUser(user.ServicePoint);

                    return RedirectToAction("ServiceSelection");
                }
                else
                {
                    var authenticationService = new JwtAuthenticationService(_configuration, issuer);
                    var token = authenticationService.GenerateToken(model);

                    if (authenticationService.ValidateToken(token))
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt");
                        return View(model);
                    }
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
        public async Task<IActionResult> SelectService(string selectedService)
        {
            string servicePoint = UserUtility.GetCurrentLoggedInUser();
            
                if (servicePoint == selectedService)
                {
                    // Perform actions when the service types match
                    return RedirectToAction("ServicePoint", "Queue");

                }
                else
                {
                // Perform actions when the service types do not match
                // Redirect to an error page or perform different logic
                return RedirectToAction("Login", "Account");
            }
            
           
        }

     /*   public static IActionResult GetLoggedInUser()
        {
            // Use 'loggedInUser' as needed, such as displaying user information
            if (loggedInUser != null)
            {
                return Json(loggedInUser);
            }
            else
            {
                return Json("No user logged in");
            }
        }*/
       
    }
}