using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Services;
using Queue_Management_System.Models;
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

        private int? GetServicePointId()
        {
            _identity = new ClaimsIdentity(User.Claims);
            var userServingPointId = _identity.HasClaim(claim => claim.Type == "ServicePointId")
               ? _identity.Claims.First(claim => claim.Type == "ServicePointId").Value
               : null;
            return Convert.ToInt32(userServingPointId);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServicePointVM>>> CheckinPage()
        {
            var services = await _queueRepository.GetServices();

            ServicePointsList servicesList = new ServicePointsList()
            {
                Services = services
            };
            return View(servicesList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddCustomerToQueue(ServicePointVM servicePointId)
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
        [Authorize(Roles = "Service Provider"), HttpGet]
        public async Task<ActionResult<IEnumerable<QueueVM>>> ServicePoint()
        {
            int? servicePointId = GetServicePointId();

            if (servicePointId != null)
            {
                var waitingCustomers = await _queueRepository.GetWaitingCustomers((int)servicePointId);//can return count = 0. In that case dont do the mycurrentserving customers function
                //TODO
                //if GetWaitingCustomers() count = 0, return view without going thru the MyCurrentServingCustomer() method
                QueueVM currentServingCustomerId = await _queueRepository.MyCurrentServingCustomer((int)servicePointId);//can return null

                QueueVMList queueList = new QueueVMList()
                {
                    /*IncomingCustomerId = incomingCustomerId,*/
                    WaitingCustomers = waitingCustomers,
                    MyCurrentServingCustomerId = currentServingCustomerId
                };
                return View(queueList);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> GetNextNumber(int id) //outgoingCustomerId
        {
            int? servicePointId = GetServicePointId();

            if (servicePointId != null)
            {
                QueueVM IncomingCustomerDetails = await _queueRepository.UpdateOutGoingAndIncomingCustomerStatus(id, (int)servicePointId);
                if (IncomingCustomerDetails == null)
                {
                    return RedirectToAction(nameof(ServicePoint));

                }
                TempData["AlertMessage"] = $"Queue Id Number {IncomingCustomerDetails.Id} Called successfully";
                return RedirectToAction(nameof(ServicePoint));
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> RecallNumber()
        {
            int? servicePointId = GetServicePointId();

            if (servicePointId != null)
            {
                QueueVM CurrentlyCalledCustomerDetails = await _queueRepository.MyCurrentServingCustomer((int)servicePointId);//change this name
                if (CurrentlyCalledCustomerDetails == null)
                {
                    TempData["AlertMessage"] = $"Error encountered while Recalling Queue Id Number";
                    return RedirectToAction(nameof(ServicePoint));

                }
                TempData["AlertMessage"] = $"Queue Id Number {CurrentlyCalledCustomerDetails.Id} ReCalled successfully";
                return RedirectToAction(nameof(ServicePoint));
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> MarkNumberASNoShow()
        {
            int? servicePointId = GetServicePointId();

            if (servicePointId != null)
            {

                await _queueRepository.MarkNumberASNoShow((int)servicePointId);
                TempData["AlertMessage"] = "Queue Id Number Marked as NoShow successfully";
                return RedirectToAction(nameof(ServicePoint));
            }
            return NotFound();

        }

        [HttpPost]
        public async Task<ActionResult> MarkNumberASFinished()
        {
            int? servicePointId = GetServicePointId();

            if (servicePointId != null)
            {

                 await _queueRepository.MarkNumberASFinished((int)servicePointId);
                TempData["AlertMessage"] = "Queue Id Number Marked as Finished successfully";
                return RedirectToAction(nameof(ServicePoint));
            }
            return NotFound();

        }

        // POST: Queue/TransferNumber
        [HttpPost]
        public async Task<ActionResult> TransferNumber(QueueVMList room)
        {
            int? currentServicePointId = GetServicePointId();

            if (currentServicePointId != null)
            {
                int servicePointIdTranser = room.MyCurrentServingCustomerId.ServicePointId;

                await _queueRepository.TransferNumber((int)currentServicePointId, servicePointIdTranser);
                TempData["AlertMessage"] = "Queue Id Number Transfered successfully";
                return RedirectToAction(nameof(ServicePoint));
            }
            return NotFound();
        }
    }
}