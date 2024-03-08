using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;

namespace Queue_Management_System.Controllers
{
    public class WaitingPage : Controller
    {
        private readonly AppDbContext _context;
        private readonly Random _random;
        Customer _customer = new Customer();
        public WaitingPage(AppDbContext context)
        {
        _context = context;
        _random = new Random();
        }
       
        public IActionResult Waiting()
        {
            // Retrieve the list of customers from the database
            List<Customer> customers = _context.customers.ToList(); 
            return View(customers);
        }
       /* public IActionResult Waiting()
        {
            var nextCustomer = _context.customers.OrderBy(c => c.Id).FirstOrDefault();
            var servicePoint = _context.ServicePoints.OrderBy(s => Guid.NewGuid()).FirstOrDefault(); // Randomly select a service point
            if ((nextCustomer != null) && (servicePoint!=null) )
            {
                var viewModel = new WaitingModel
                {
                    TicketNumber = nextCustomer?.Id,
                    ServicePoint = servicePoint?.Id
                };
                return View(viewModel);

            }
            return View();
        }*/
      

    }
}
