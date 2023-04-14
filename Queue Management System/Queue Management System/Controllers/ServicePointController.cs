using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;
using Queue_Management_System.Models.Data;
using ServiceProvider = Queue_Management_System.Models.ServiceProvider;
namespace Queue_Management_System.Controllers
{
    public class ServicePointController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public ServicePointController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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

        public IActionResult Queue()
        {
            // Retrieve the current service point ID from the session variable
            var servicePointId = HttpContext.Session.GetInt32("ServicePointId");

            // Query the database to retrieve the list of customers with the selected service point ID
            var customers = _dbContext.Customers
                .Where(q => q.ServicePointId == servicePointId && q.Completed == false)
                .ToList();

            // Pass the list of customers to the view
            return View(customers);
        }

        public IActionResult GetNextNumber()
        {
            // Retrieve the current service point ID from the session variable
            var servicePointId = HttpContext.Session.GetInt32("ServicePointId");

            // Query the database to retrieve the next customer in the queue with the selected service point ID and status "waiting"
            var nextCustomer = _dbContext.Customers
                .FirstOrDefault(q => q.ServicePointId == servicePointId && q.Status == "Waiting");

            // If there is no next customer, display a message to the service provider
            if (nextCustomer == null)
            {
                TempData["error"] = "No Customers queued to Your room";
                return RedirectToAction("Queue");
            }

            // Update the customer's status to "in progress"
            nextCustomer.Status = "In Progress";
            nextCustomer.StartServiceTime = DateTime.Today.ToUniversalTime().Add(DateTime.Now.TimeOfDay);
            nextCustomer.CallTime = DateTime.Now.ToUniversalTime();
            nextCustomer.IsCalled = true;
            TempData["success"] = "Next Customer called Successfully";
            _dbContext.SaveChanges();
            return RedirectToAction("Queue");

            // Pass the customer's ticket number to the view
            // ViewData["TicketNumber"] = nextCustomer.Id;

            // return View(new { Id = servicePointId });
        }

        public IActionResult RecallNumber(int Id)
        {
            var servicePointId = HttpContext.Session.GetInt32("ServicePointId");

            var customer = _dbContext.Customers.FirstOrDefault(q => q.Id == Id || q.Status == "In Progress");

            if (customer != null)
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

        public IActionResult MarkAsNoShow(int Id)
        {
            var servicePointId = HttpContext.Session.GetInt32("ServicePointId");
            // Get the next customer in the queue for the selected service point with status "in progress"
            var customer = _dbContext.Customers
                .Where(q => q.Id == Id)
                .FirstOrDefault();

            if (customer == null)
            {
                // If no customer is currently being served, display an error message
                TempData["error"] = "The Customer is currently not being served.";
                return View("Index");
            }

            // Update the customer's status to "no show" and remove them from the queue
            customer.Status = "No Show";
            customer.NoShow = true;
            // Display a success message with the customer's ticket number
            TempData["success"] = "Customer Marked As No Show Successfully";
            _dbContext.SaveChanges();
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

            // If customer is not found, return to queue view
            TempData["error"] = "Transferred Failed";
            return RedirectToAction("Queue");
        }

    }
}