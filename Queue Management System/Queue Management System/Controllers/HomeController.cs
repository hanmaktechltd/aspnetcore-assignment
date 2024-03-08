using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;
using System.Diagnostics;

namespace Queue_Management_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly Random _random;
        Customer _customer = new Customer();
        public HomeController(ILogger<HomeController> logger,AppDbContext context)
         {
             _logger = logger;
            _context = context;
        } 
        public IActionResult Index()
        {
            List<WaitingModel> waitingModels = _context.waitingModels.ToList();

            return View(waitingModels);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult CheckIn()
        {
            return View();
        }
        public IActionResult WaitingPage()
        {
            var CustWaiting = _context.waitingModels.ToList();
            var viewModels = new List<WaitingModel>();
            try
            {
                foreach (var waits in CustWaiting)
                {
                    var viewModel = new WaitingModel();
                    viewModel.TicketNumber = waits.TicketNumber;
                    viewModel.ServicePoint = waits.ServicePoint;
                    viewModel.ServicePointName = waits.ServicePointName;
                    viewModels.Add(viewModel);
                }
            }
            catch (Exception ex)
            {

            }
            return View(viewModels);

        }
        public IActionResult ServicePoints()
        {
            List<ServicePoint> _servicePoints = _context.ServicePoints.ToList();
            return View(_servicePoints);
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Delete(int id)
        {
            var servicepoint = _context.ServicePoints.FirstOrDefault(x => x.Id == id);
            if (servicepoint != null)
            {
                _context.ServicePoints.Remove(servicepoint);
                _context.SaveChanges();
            }
            return RedirectToAction("ServicePoints", "Home");
        }
        public IActionResult Edit(int id)
        {
            var servicePoint = _context.ServicePoints.FirstOrDefault(sp => sp.Id == id);

            if (servicePoint == null)
            {
                return NotFound(); 
            }
            return View(servicePoint);
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Logout()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult WaitingPage1() 
        {
            List<Customer> customers = _context.customers.ToList();
            var nextCustomer = _context.customers.OrderBy(c => c.Id).FirstOrDefault();
            try
            {
                if ((nextCustomer != null) && (nextCustomer.Status == "Waiting"))
                {
                    var servicePoint = _context.ServicePoints.OrderBy(s => Guid.NewGuid()).FirstOrDefault();
                    if ((servicePoint != null) && servicePoint.Status =="Open")
                    {
                        var viewModel = new WaitingModel
                        {
                            TicketNumber = nextCustomer?.Id,
                            ServicePoint = servicePoint?.Id,
                            ServicePointName = servicePoint?.Name,
                            
                        };
                        _context.waitingModels.Add(viewModel);
                        _context.SaveChanges();
                        //update Service point status
                       // servicePoint.Status = "Busy";
                        //_context.ServicePoints.Update(servicePoint);
                        //_context.SaveChanges();
                        return View(viewModel);
                    }
                }
            }
            catch (Exception ex)
            {
            
            }
            return View();
           
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}