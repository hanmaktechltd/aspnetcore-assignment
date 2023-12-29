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


        // GET: Queue/ServicePoint
        [Authorize, HttpGet]
        public async Task<IActionResult> ServicePoint()
        {
            int? servicePointId = GetServicePointId();
            if (servicePointId != null)
            {
                var waitingCustomers = await _queueRepository.GetWaitingCustomers((int)servicePointId);
                QueueM currentCustomerId = await _queueRepository.CurrentServingCustomer((int)servicePointId);
                var services = await _queueRepository.GetServices();
                QueueMList queueList = new QueueMList()
                {
                    WaitingCustomers = waitingCustomers,
                    CurrentServingCustomerId = currentCustomerId,
                    Services = services,
                };
                return View(queueList);
            }
            return NotFound();
        }


        private int? GetServicePointId()
        {
            _identity = new ClaimsIdentity(User.Claims);

            var servicePointIdClaim = _identity.Claims.FirstOrDefault(claim => claim.Type == "ServicePointId");

            if (servicePointIdClaim != null && int.TryParse(servicePointIdClaim.Value, out int servicePointId))
            {
                return servicePointId;
            }

            return null;
        }

 



    }
}
