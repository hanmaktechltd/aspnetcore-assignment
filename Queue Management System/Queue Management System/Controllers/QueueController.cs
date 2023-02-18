using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Services;
using Queue_Management_System.Models;
using System.Net;

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

        // GET: Queue/WaitingPage
        [HttpGet]
        public async Task<IActionResult> WaitingPage()
        {
            var calledCustomers = await _queueRepository.GetCalledCustomers();
            return View(calledCustomers);
        }

        // GET: Queue/ServicePoint
        [Authorize(Roles = "Service Provider"), HttpGet]
        public async Task<ActionResult<IEnumerable<QueueVM>>> ServicePoint(int incomingCustomerId)
        {

            foreach (var claim in User.Claims)
            {
                var userServingPointId = @claim.Value;

                var waitingCustomers = await _queueRepository.GetWaitingCustomers(userServingPointId);

                var currentServingCustomerId = await _queueRepository.MyCurrentServingCustomer(userServingPointId);

                return View(new QueueVM2()
                {
                    IncomingCustomerId = incomingCustomerId,
                    WaitingCustomers = waitingCustomers,
                    MyCurrentServingCustomerId = currentServingCustomerId
                });
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> GetNextNumber(int id) //outgoingCustomerId
        {
            foreach (var claim in User.Claims)
            {
                var serviceProviderId = @claim.Value;

                var IncomingCustomerDetails = await _queueRepository.UpdateOutGoingAndIncomingCustomerStatus(id, serviceProviderId);
                if (IncomingCustomerDetails == null)
                {
                    return RedirectToAction(nameof(ServicePoint));

                }
                TempData["AlertMessage"] = $"Queue Id Number {IncomingCustomerDetails.Id} Called successfully";
                return RedirectToAction(nameof(ServicePoint), new { incomingCustomerId = IncomingCustomerDetails.Id });
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> RecallNumber()
        {
            foreach (var claim in User.Claims)
            {
                var serviceProviderId = @claim.Value;

                var CurrentlyCalledCustomerDetails = await _queueRepository.GetCurentlyCalledNumber(serviceProviderId);//change this name
                if (CurrentlyCalledCustomerDetails == null)
                {
                    return RedirectToAction(nameof(ServicePoint));

                }
                TempData["AlertMessage"] = $"Queue Id Number {CurrentlyCalledCustomerDetails.Id} ReCalled successfully";
                return RedirectToAction(nameof(ServicePoint), new { incomingCustomerId = CurrentlyCalledCustomerDetails.Id });
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> MarkNumberASNoShow()
        {
            foreach (var claim in User.Claims)
            {
                var serviceProviderId = @claim.Value;

                var IncomingCustomerDetails = await _queueRepository.MarkNumberASNoShow(serviceProviderId);
                TempData["AlertMessage"] = "Queue Id Number Marked as NoShow successfully";
                return RedirectToAction(nameof(ServicePoint));
            }
            return NotFound();

        }

        [HttpPost]
        public async Task<ActionResult> MarkNumberASFinished()
        {
            foreach (var claim in User.Claims)
            {
                var serviceProviderId = @claim.Value;

                var IncomingCustomerDetails = await _queueRepository.MarkNumberASFinished(serviceProviderId);
                TempData["AlertMessage"] = "Queue Id Number Marked as Finished successfully";
                return RedirectToAction(nameof(ServicePoint));
            }
            return NotFound();

        }

        // POST: Queue/TransferNumber
        [HttpPost]
        public async Task<ActionResult> TransferNumber(int id, QueueVM2 room)
        {
            foreach (var claim in User.Claims)
            {
                var servicePointid = room.MyCurrentServingCustomerId.ServicePointId;
                var serviceProviderId = @claim.Value;

                var IncomingCustomerDetails = await _queueRepository.TransferNumber(serviceProviderId, servicePointid);
                TempData["AlertMessage"] = "Queue Id Number Transfered successfully";
                return RedirectToAction(nameof(ServicePoint));
            }
            return NotFound();
        }
    }
}