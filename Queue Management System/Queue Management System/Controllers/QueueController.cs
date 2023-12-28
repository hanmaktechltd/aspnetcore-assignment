using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;
using Queue_Management_System.Services;
using System.Net;
using System.Security.Claims;

namespace Queue_Management_System.Controllers
{
    public class QueueController : Controller
    {
        private readonly IQueueRepository _queueRepository;
        private ClaimsIdentity _identity;
        public QueueController(IQueueRepository queueRepository)
        {
            _queueRepository = queueRepository;
        }

        [HttpGet]
        public async Task<IActionResult> CheckinPage()
        {
            var services = await _queueRepository.GetServices();
            return View(services);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddCustomerToQueue(ServicePointM servicePointId)
        {
            await _queueRepository.AddCustomerToQueue(servicePointId);
            return RedirectToAction(nameof(CheckinPage));
        }



        // GET: Queue/WaitingPage
        [HttpGet]
        public async Task<IActionResult> WaitingPage()
        {
            var calledCustomers = await _queueRepository.GetCalledCustomers();
            return View(calledCustomers);
        }




        [Authorize, HttpGet]
        public IActionResult ServicePoint()
        {
            return View();
        }


    }
}
