using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;
//using System.Net;

namespace Queue_Management_System.Controllers
{
    public class ServicePointController : Controller
    {
        private readonly AppDbContext _context;
        public ServicePointController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var servicepoints = _context.ServicePoints.ToList();
            return View(servicepoints);
        }        
        [HttpPost]
        public IActionResult Create(ServicePoint model)
        {
            //Find the last record and increment it by 1
            int lastId=_context.ServicePoints.OrderByDescending(x=>x.Id).Select(x=>x.Id).FirstOrDefault();
            model.Id = lastId + 1;
            _context.ServicePoints.Add(model);
            _context.SaveChanges();
            return RedirectToAction("ServicePoints", "Home");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var servicepoint = _context.ServicePoints.FirstOrDefault(x => x.Id == id);
            if (servicepoint == null)
            {
                return NotFound(); 
            }
            return View(servicepoint);
        }
        [HttpPost]
        public IActionResult Edit(ServicePoint model)
        {
            _context.ServicePoints.Update(model);
            _context.SaveChanges();
            return RedirectToAction("ServicePoints", "Home");
        }
        [HttpPost]
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

        public IActionResult QueueItem()
        {
            List<QueueItem> queues = _context.QueueItems.ToList();

            return View(queues);
        }
        public IActionResult GetNextNumber(int servicePointId)
        {
            // Logic to get the next number for the given service point
            var nextNumber = _context.QueueItems.Where(q => q.ServicePoint == servicePointId && !q.IsCalled)
                                                .OrderBy(q => q.TicketNumber)
                                                .FirstOrDefault();
            if (nextNumber != null)
            {
                nextNumber.IsCalled = true;
                _context.SaveChanges();
            }
            return RedirectToAction("QueueItem", "ServicePoint");
        }
        public IActionResult RecallNumber(int ticketNumber)
        {
            // Logic to recall a number
            var ticket = _context.QueueItems.FirstOrDefault(q => q.TicketNumber == ticketNumber);
            if (ticket != null)
            {
                ticket.IsCalled = false;
                _context.SaveChanges();
            }
            return RedirectToAction("ServicePoints", "Home");
        }
        public IActionResult MarkAsFinished(int ticketNumber)
        {
            // Logic to mark a number as finished
            var ticket = _context.QueueItems.FirstOrDefault(q => q.TicketNumber == ticketNumber);
            if (ticket != null)
            {
                ticket.Finished = true;
                _context.SaveChanges();
            }
            return RedirectToAction("ServicePoints", "Home");
        }
        public IActionResult NoShow(int ticketNumber)
        {
            // Logic to mark a number as finished
            var ticket = _context.QueueItems.FirstOrDefault(q => q.TicketNumber == ticketNumber);
            if (ticket != null)
            {
                ticket.NoShow = true;
                _context.SaveChanges();
            }
            return RedirectToAction("ServicePoints", "Home");
        }
        public IActionResult TransferNumber(int ticketNumber, int newServicePointId)
        {
            // Logic to transfer a number to another service point
            var ticket = _context.QueueItems.FirstOrDefault(q => q.TicketNumber == ticketNumber);
            if (ticket != null)
            {
                ticket.ServicePoint = newServicePointId;
                _context.SaveChanges();
            }
            return RedirectToAction("ServicePoints", "Home");
        }
        public IActionResult ViewQueue(int servicePointId)
        {
            // Retrieve queue data from the database for the given service point
            var queueData = _context.QueueItems.Where(q => q.ServicePoint == servicePointId).ToList();
            return View(queueData);
        }
    }
}
