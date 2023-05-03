using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Queue_Management_System.Models;
using Queue_Management_System.Services;
using ServiceProvider = Queue_Management_System.Models.ServiceProvider;
namespace Queue_Management_System.Controllers
{
    public class ServicePointController : Controller
    {
        private readonly IServicePointRepository _servicePointRepository;
        private readonly IConfiguration _configuration;

        public ServicePointController(IServicePointRepository servicePointRepository, IConfiguration configuration)
        {
            _configuration = configuration;
            _servicePointRepository = servicePointRepository;
        }

        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync(string Name, string Password)
        {
            if (ModelState.IsValid)
            {
                var provider = await _servicePointRepository.Login(Name, Password);
                if (provider is not null)
                {
                    HttpContext.Session.SetInt32("ServicePointId", provider.ServicePointId);
                    TempData["success"] = "Login Successfully";
                    return RedirectToAction("SelectService", new { Id = provider.Id });
                }
                TempData["error"] = "Invalid Login Credentials";
            }
            return View();
        }


        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync("ServicePointAuthentication");
            TempData["success"] = "Logout successfull";
            return RedirectToAction("Login");
        }

        [Authorize(AuthenticationSchemes = "ServicePointAuthentication")]
        public IActionResult SelectService()
        {
            return View("Menu");
        }

        public async Task<IActionResult> Queue()
        {
            // Retrieve the current service point ID from the session variable
            var servicePointId = HttpContext.Session.GetInt32("ServicePointId");
            var customers = await _servicePointRepository.GetQueueStatus(servicePointId);
            if (customers.Count() is 0)
            {
                TempData["Error"] = "No Customers Waiting To Be Served";
                return RedirectToAction("SelectService");
            }
            // Pass the list of customers to the view
            return View(customers);
        }

        public async Task<IActionResult> GetNextNumber()
        {
            var servicePointId = HttpContext.Session.GetInt32("ServicePointId");
            var nextCustomer = await _servicePointRepository.GetNextNumber(servicePointId);

            if (nextCustomer is null)
            {
                TempData["error"] = "No Customers queued to Your room";
                return RedirectToAction("Queue");
            }
            TempData["success"] = "Next Customer called Successfully";
            return RedirectToAction("Queue");
        }

        public async Task<IActionResult> RecallNumber(int Id)
        {
            var servicePointId = HttpContext.Session.GetInt32("ServicePointId");
            var recalledCustomer = await _servicePointRepository.RecallNumber(Id, servicePointId);
            if (recalledCustomer is not null)
            {
                TempData["success"] = "Customer Recalled Successfully";
                return RedirectToAction("Queue");
            }
            TempData["error"] = "An error occurred while recalling the customer";
            return RedirectToAction("Queue");
        }

        public async Task<IActionResult> MarkAsNoShow(int Id)
        {
            var servicePointId = HttpContext.Session.GetInt32("ServicePointId");
            await _servicePointRepository.MarkAsNoShow(Id, servicePointId);

            // Display a success message with the customer's ticket number
            TempData["success"] = "Customer Marked As No Show Successfully";
            return RedirectToAction("Queue");
        }
        public async Task<IActionResult> MarkAsFinished(int Id)
        {
            await _servicePointRepository.MarkAsFinished(Id);
            TempData["success"] = "Customer Marked As Finished Successfully";
            return RedirectToAction("Queue");
        }

        // Controller action to handle transfer request
        [HttpGet]
        public async Task<IActionResult> Transfer(int Id)
        {
            var servicePointId = HttpContext.Session.GetInt32("ServicePointId");
            var model = await _servicePointRepository.TransferNumber(Id, servicePointId);
            return View(model);
        }

        // Controller action to handle transfer form submission
        // to be modified
        [HttpPost]
        public async Task<IActionResult> Transfer(int Id, int servicePointId)
        {
            await _servicePointRepository.TransferPost(Id, servicePointId);
            TempData["success"] = "Customer transfered successfully";
            return RedirectToAction("Queue");
        }

    }
}