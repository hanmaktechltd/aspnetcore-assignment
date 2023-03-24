using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        public IActionResult Index()
        {
            return View("Authenticate");
        }

        public IActionResult Login(string Name, string Password)
        {
            ViewBag.LoginStatus = "";

            if (ModelState.IsValid)
            {
                ViewBag.LoginStatus = "";

                var UserCheck = _dbContext.ServiceProviders.FirstOrDefault
                    (a => a.Name == Name && a.Password == Password);

                if (UserCheck == null)
                {
                    ViewBag.LoginStatus = "Invalid Login. User not found";
                }
                else
                {
                    HttpContext.Session.SetInt32("ServicePointId", UserCheck.ServicePointId);
                    return RedirectToAction("SelectService", new { Id = UserCheck.Id });
                }
            }
            return View();
        }

        public IActionResult SelectServicePoint()
        {
            IEnumerable<SelectListItem> getExpenseCategoryList =
                _dbContext.ServicePoints.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });

            ViewBag.PopulateServicePoint = getExpenseCategoryList;
            return View();
        }

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
                ViewData["Message"] = "There are no customers in the queue.";
                return View();
            }

            // Update the customer's status to "in progress"
            nextCustomer.Status = "In Progress";
            nextCustomer.StartServiceTime = DateTime.Today.ToUniversalTime().Add(DateTime.Now.TimeOfDay);
            nextCustomer.CallTime = DateTime.Now.ToUniversalTime();
            nextCustomer.IsCalled = true;
            _dbContext.SaveChanges();

            // Pass the customer's ticket number to the view
            ViewData["TicketNumber"] = nextCustomer.Id;
            // HttpContext.Session.SetInt32("Id", nextCustomer.Id);

            // Display the view with the customer's ticket number
            return View(new { Id = servicePointId });
        }

        [HttpPost]
        public IActionResult RecallNumber(int Id)
        {
            var servicePointId = HttpContext.Session.GetInt32("ServicePointId");

            var customer = _dbContext.Customers.FirstOrDefault(q => q.Id == Id && q.Status == "In Progress");

            if (customer != null)
            {
                // Update the customer's status to "waiting"
                customer.Status = "Waiting";
                _dbContext.SaveChanges();

                // Display the customer's ticket number on the screen
                ViewBag.Message = "Recalled customer with ticket number: " + customer.Id;
            }
            else
            {
                // Display a message indicating that there are no customers currently being served
                ViewBag.Message = "No customers currently being served at this service point.";
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
                ViewBag.Message = "No customer currently being served.";
                return View("Index");
            }

            // Update the customer's status to "no show" and remove them from the queue
            customer.Status = "No Show";
            customer.NoShow = true;
            // _dbContext.Customers.Remove(customer);
            _dbContext.SaveChanges();

            // Display a success message with the customer's ticket number
            ViewBag.Message = "Customer " + customer.Id + " marked as no show.";
            return RedirectToAction("SelectService");
        }

        [HttpPost]
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
                _dbContext.SaveChanges();

                // Remove the customer from the queue
                // _dbContext.Customers.Remove(customer);
                // _dbContext.SaveChanges();

                return RedirectToAction("SelectService");
            }

            return RedirectToAction("SelectService");
        }

        // Controller action to handle transfer request
        public IActionResult Transfer(int Id)
        {
            // Query for the customer with the given ID
            var customer = _dbContext.Customers.FirstOrDefault(c => c.Id == Id);

            if (customer != null)
            {
                // Query for all available service points
                var servicePoints = _dbContext.ServicePoints.ToList();

                // Create a list of select items for the dropdown list
                var servicePointSelectList = new SelectList(servicePoints, "Id", "Name");

                // Add a "Select Service Point" option at the top of the list
                servicePointSelectList = new SelectList(servicePointSelectList.Items, "Value", "Text", null);

                // Pass the customer and service point select list to the view
                ViewData["Customer"] = customer;
                ViewData["ServicePointSelectList"] = servicePointSelectList;

                return View();
            }

            // If customer is not found, return to queue view
            return RedirectToAction("Queue");
        }

        // Controller action to handle transfer form submission
        [HttpPost]
        public IActionResult Transfer(int customerId, int servicePointId)
        {
            // Query for the customer with the given ID
            var customer = _dbContext.Customers.FirstOrDefault(c => c.Id == customerId);

            if (customer != null)
            {
                // Update the customer's service point ID and status in the database
                customer.ServicePointId = servicePointId;
                customer.Status = "Waiting";
                _dbContext.Customers.Update(customer);
                _dbContext.SaveChanges();

                // Redirect back to the queue view for the original service point
                return RedirectToAction("Queue");
            }

            // If customer is not found, return to queue view
            return RedirectToAction("Queue");
        }

    }
}