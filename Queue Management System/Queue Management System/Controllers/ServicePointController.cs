using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;
using Queue_Management_System.Models.Data;
using Queue_Management_System.Services;
using ServiceProvider = Queue_Management_System.Models.ServiceProvider;
namespace Queue_Management_System.Controllers
{
    public class ServicePointController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IServicePointRepository _servicePointRepository;

        public ServicePointController(ApplicationDbContext dbContext, IServicePointRepository servicePointRepository)
        {
            _dbContext = dbContext;
            _servicePointRepository = servicePointRepository;
        }

        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync(string Name, string Password)
        {
            if (ModelState.IsValid)
            {
                var UserCheck = _dbContext.ServiceProviders.FirstOrDefault
                    (a => a.Name == Name && a.Password == Password);

                if (UserCheck == null)
                {
                    TempData["error"] = "Invalid Login Credentials";
                }
                else
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, UserCheck.Password),
                        new Claim(ClaimTypes.Role, "serviceProvider")
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, "ServicePointAuthentication");

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };

                    await HttpContext.SignInAsync(
                        "ServicePointAuthentication",
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    HttpContext.Session.SetInt32("ServicePointId", UserCheck.ServicePointId);
                    TempData["success"] = "Login Successfully";
                    return RedirectToAction("SelectService", new { Id = UserCheck.Id });
                }
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
            // Pass the list of customers to the view
            return View(customers);
        }

        public async Task<IActionResult> GetNextNumber()
        {
            var servicePointId = HttpContext.Session.GetInt32("ServicePointId");
            var nextCustomer = await _servicePointRepository.GetNextNumber(servicePointId);

            if (nextCustomer == null)
            {
                TempData["error"] = "No Customers queued to Your room";
                return RedirectToAction("Queue");
            }
            else
            {
                TempData["success"] = "Next Customer called Successfully";
                return RedirectToAction("Queue");
            }
        }

        public IActionResult RecallNumber(int Id)
        {
            var servicePointId = HttpContext.Session.GetInt32("ServicePointId");

            var customer = _dbContext.Customers.FirstOrDefault(q => q.Id == Id || q.Status == "In Progress");

            if (customer is not null)
            {
                // Update the customer's status to "waiting"
                customer.Status = "Waiting";
                TempData["success"] = "Customer Recalled Successfully";
                _dbContext.SaveChanges();
            }
            else
            {
                // Display a message indicating that there are no customers currently being served
                TempData["error"] = "No customers currently being served at this service point";
            }
            return RedirectToAction("Queue", new { Id = servicePointId });
        }

        public async Task<IActionResult> MarkAsNoShow(int Id)
        {
            var servicePointId = HttpContext.Session.GetInt32("ServicePointId");
            await _servicePointRepository.MarkAsNoShow(Id, servicePointId);

            // Display a success message with the customer's ticket number
            TempData["success"] = "Customer Marked As No Show Successfully";
            return RedirectToAction("Queue");
        }
        public IActionResult MarkAsFinished(int Id)
        {
            var servicePointId = HttpContext.Session.GetInt32("ServicePointId");
            // Query for the next customer with the selected service point ID and status "in progress"
            var customer = _dbContext.Customers
                .Where(q => q.Id == Id && q.Status == "In Progress")
                .FirstOrDefault();

            if (customer != null)
            {
                // Update the customer's status to "finished"
                customer.EndServiceTime = DateTime.Today.ToUniversalTime().Add(DateTime.Now.TimeOfDay);
                customer.Status = "Finished";
                customer.Completed = true;
                TempData["success"] = "Customer Marked As Finished Successfully";
                _dbContext.SaveChanges();
                return RedirectToAction("Queue");
            }

            return RedirectToAction("Queue");
        }

        // Controller action to handle transfer request
        [HttpGet]
        public IActionResult Transfer(int Id)
        {
            var servicePointId = HttpContext.Session.GetInt32("ServicePointId");
            // Query for the customer with the given ID
            var customer = _dbContext.Customers
                .Where(c => c.Id == Id)
                .FirstOrDefault();

            if (customer == null)
            {
                // If customer is not found, return to queue view
                TempData["error"] = "Error while transferring customer";
                return RedirectToAction("Queue");
            }
            // Retrieve the list of available service points (excluding the current service point)
            var servicePoints = _dbContext.ServicePoints.Where(sp => sp.Id != servicePointId).ToList();
            var model = new TransferView
            {
                Customers = customer,
                ServicePoints = servicePoints
            };
            return View(model);
        }

        // Controller action to handle transfer form submission
        [HttpPost]
        public IActionResult Transfer(int Id, int servicePointId)
        {
            // Query for the customer with the given ID
            var customer = _dbContext.Customers.FirstOrDefault(c => c.Id == Id);

            if (customer != null)
            {
                // Update the customer's service point ID and status in the database
                customer.ServicePointId = servicePointId;
                customer.Status = "Waiting";
                _dbContext.Customers.Update(customer);
                TempData["success"] = "Customer transfered successfully";
                _dbContext.SaveChanges();

                // Redirect back to the queue view for the original service point
                return RedirectToAction("Queue");
            }

            return NotFound();
            // If customer is not found, return to queue view
            TempData["error"] = "Transferred Failed";
            return RedirectToAction("Queue");
        }

    }
}