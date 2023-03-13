using FastReport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Queue_Management_System.Models;
using Queue_Management_System.Models.ViewModels;
using Queue_Management_System.Repository;
using Queue_Management_System.Services;

namespace Queue_Management_System.Controllers
{
    public class QueueController : Controller
    {
        private readonly QueueRepository _repository;
        private readonly QueueService _queueService;

        public QueueController(
             QueueRepository repository, QueueService queueService
            )
        {
            _repository = repository;
            _queueService = queueService;
        }

        [HttpGet]
        public IActionResult CheckinPage()
        {
            var servicePoints = _repository.getServicePoints();
            List<SelectListItem> servicePointListItems = servicePoints.Select(sp => new SelectListItem
            {
                Value = sp.Id.ToString(),
                Text = sp.Name
            }).ToList();

            ViewBag.ServicePoints = servicePointListItems;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CheckinPage(CheckInViewModel checkInRequest)
        {
            var selectedServicePoint = _repository.getServicePointById(checkInRequest.ServiceId);
            //check waiting line
            var customersInQueue = _queueService.GetCustomersInQueueByServicePoint(checkInRequest.ServiceId);
            var totalCustomersServed = _queueService.GetTotalCustomersServed();
            var TicketNumber = "TKT" + totalCustomersServed; 
            //add customer to queue
            Customers customer = new Customers
            {
                Name = checkInRequest.Name,
                ServicePoint = selectedServicePoint,
                Status="WAITING",
                TicketNumber= TicketNumber,
                TimeIn = DateTime.Now.ToUniversalTime(),
            };

            await _repository.CreateCustomer(customer);
            await _repository.SaveChanges();

            List<TicketViewModel> tickets = new List<TicketViewModel>();
            TicketViewModel ticket = new TicketViewModel
            {
                Name = checkInRequest.Name,
                TicketNumber = TicketNumber,
                CustomersInQueue = customersInQueue,
                Message = "It is a pleasure serving you"
            };
            tickets.Add(ticket);
            var pdf=_queueService.GenerateTicket(tickets);
            HttpContext.Session.SetString("TicketNumber", TicketNumber);
            return File(pdf, "application/pdf","myreport.pdf");
            //return View("WaitingPage", tickets);
        }
     

        [HttpGet]
        public IActionResult WaitingPage()
        {
            var ticketNumber = HttpContext.Session.GetString("TicketNumber");
            var customers = _repository.RecallCustomer(ticketNumber);
            return View(customers);
        }



        [HttpGet]
        [Authorize(Roles = "ServiceProvider")]
        public IActionResult ServicePoint(int servicePointId)
        {
            if (servicePointId == 0)
            {
                return RedirectToAction("SelectServicePoint", "Account");
            }
            var customers = _repository.GetCustomersinQueueByServicePoint(servicePointId);
            var CurrentCustomer = _repository.GetCurrentCustomer(servicePointId);
            var CustomerQueue = Tuple.Create(customers, CurrentCustomer);
            return View(CustomerQueue);
        }
        public IActionResult EditStatus(int Id)
        {
            List<SelectListItem> statusOptions = new List<SelectListItem>();
            statusOptions.Add(new SelectListItem { Value = "0", Text = "Select Status", Selected=true, Disabled=true });
            statusOptions.Add(new SelectListItem { Value = "NOSHOW", Text = "No Show" });
            statusOptions.Add(new SelectListItem { Value = "FINISHED", Text = "Finished" });
            ViewBag.StatusOptions = statusOptions;


            var customer = _repository.GetCustomerById(Id);
            return View(customer);
        }
        [HttpPost]
        public IActionResult EditStatus(Customers customer)
        {
            _repository.updateCustomerStatus(customer);
            var servicePointId = HttpContext.Session.GetInt32("ServicePointId");
            var customers = _repository.GetCustomersinQueueByServicePoint(servicePointId);
            var CurrentCustomer = _repository.GetCurrentCustomer(servicePointId);
            var CustomerQueue = Tuple.Create(customers, CurrentCustomer);
            return View("ServicePoint", CustomerQueue);
        }

        public IActionResult TransferCustomer(int Id)
        {
           

            var servicePoints = _repository.getServicePoints();
            List<SelectListItem> servicePointListItems = servicePoints.Select(sp => new SelectListItem
            {
                Value = sp.Id.ToString(),
                Text = sp.Name,
                Selected = sp.Id == 3
            }).ToList();

            ViewBag.ServicePoints = servicePointListItems;

            var customer = _repository.GetCustomerById(Id);
            return View(customer);
        }
        [HttpPost]
        public IActionResult TransferCustomer(Customers customer)
        {
            _repository.TransferCustomerStatus(customer);
            var servicePointId = HttpContext.Session.GetInt32("ServicePointId");
            var customers = _repository.GetCustomersinQueueByServicePoint(servicePointId);
            var CurrentCustomer = _repository.GetCurrentCustomer(servicePointId);
            var CustomerQueue = Tuple.Create(customers, CurrentCustomer);
            return View("ServicePoint", CustomerQueue);
        }
        [HttpPost]
        public IActionResult RecallCustomer(string TicketNumber)
        {
            
            var servicePointId = HttpContext.Session.GetInt32("ServicePointId");
            var customers = _repository.GetCustomersinQueueByServicePoint(servicePointId);
            var CurrentCustomer = _repository.RecallCustomer(TicketNumber);
            var CustomerQueue = Tuple.Create(customers, CurrentCustomer);
            return View("ServicePoint", CustomerQueue);
        }

    }
}
