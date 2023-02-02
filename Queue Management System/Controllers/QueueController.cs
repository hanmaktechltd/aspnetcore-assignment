using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Contracts;
using Queue_Management_System.Models;
using Queue_Management_System.Repositories;

namespace Queue_Management_System.Controllers
{
    public class QueueController : Controller
    {
        private readonly IQueueRepository _queueRepository;
        public QueueController(IQueueRepository queueRepository)
        {
            _queueRepository = queueRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServicePointVM>>> CheckinPage()
        {
            var services = await _queueRepository.GetServices();
            return View(services);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddCustomerToQueue(ServicePointVM customer)
        {
            await _queueRepository.AddCustomerToQueue(customer);
            return RedirectToAction(nameof(CheckinPage));
        }



















        [HttpGet]
        public IActionResult WaitingPage()
        {
            return View();
        }



        [Authorize, HttpGet]
        public IActionResult ServicePoint()
        {
            return View("Index");
        }


    }
}
