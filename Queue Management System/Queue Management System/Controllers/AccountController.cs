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
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration, ITicketService ticketService, DbOperationsRepository dbOperationsRepository)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration)); 
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
        { if (ModelState.IsValid)
            {
                string servicePoint = "";
                try
                {

                    var user = await _dbOperationsRepository.LoginAsync(model.UsernameOrEmail, model.Password);
                var admin = await _dbOperationsRepository.AdminLoginAsync(model.UsernameOrEmail, model.Password);

                if (admin != null)
                {
                    HandleSuccessfulLogin(model.UsernameOrEmail, isAdmin: true);
                    return RedirectToAction("Dashboard", "Admin");
                }
                else if (user != null)
                {
                        servicePoint = user.ServicePoint;

                    HandleSuccessfulLogin(model.UsernameOrEmail, isAdmin: false);
                    return RedirectToAction("ServiceSelection");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt");
                    return View(model);
                }

                void HandleSuccessfulLogin(string username, bool isAdmin)
                {
                    UserUtility.issuer = username;
                    var authenticationService = new JwtAuthenticationService(_configuration, username);
                    var token = authenticationService.GenerateToken(model);

                    if (authenticationService.ValidateToken(token))
                    {
                        if (isAdmin)
                        {
                            UserUtility.IsAdmin = true;
                        }
                        else
                        {
                            UserUtility.SetLoggedInUser(servicePoint);
                            UserUtility.IsAuthorized = true;
                        }
                    }
                }
            }
        catch (Exception ex)
        {
                ModelState.AddModelError(string.Empty, "An error occurred during login");
                Console.WriteLine($"Login Error: {ex.Message}");
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
        public async Task<IActionResult> SelectService(string selectedService)
        {
            string servicePoint = UserUtility.GetCurrentLoggedInUser();
            
                if (servicePoint == selectedService)
                {
                    return RedirectToAction("ServicePoint", "Queue");

                }
                else
                {
                return RedirectToAction("Login", "Account");
            }
            
           
        }

    
       
    }
}