using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Data;

namespace Queue_Management_System.Controllers
{
    public class QueueController : Controller
    {
        private readonly QueueDbContext _dbContext;
        public QueueController(QueueDbContext dbContext)
        {
            _dbContext= dbContext;  
        }

        [HttpGet]
        public IActionResult Index()
        {
            
            return View();
        }



        [HttpGet]
        public IActionResult Customers()
        {
            var customer = _dbContext.Customers.Where(n=>n.Status=="Pending").ToList().Take(1);
            return View(customer.OrderBy(n=>n.TicketNumber));
        }



        //[Authorize, HttpGet]
        [HttpGet]
        public IActionResult ServicePoint()
        {
            var customer = _dbContext.Customers.Where(n => n.Status == "Pending").ToList().Take(1);
            return View(customer.OrderBy(n => n.TicketNumber));
        }


    }
}
