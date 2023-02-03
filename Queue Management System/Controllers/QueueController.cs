using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
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
        public async Task< IActionResult> WaitingPage()
        {
            foreach (var claim in User.Claims)
            {
                var userServingPointId = @claim.Value;
              /*  var currentServingCustomerId = await _queueRepository.MyCurrentServingCustomer(userServingPointId);*/
               var calledCustomers = await _queueRepository.GetCalledCustomers();
                return View(calledCustomers);
            }
            return NotFound();
        }

        [Authorize(Roles = "Service Provider"), HttpGet]
        public async Task<ActionResult<IEnumerable<QueueVM>>> ServicePoint(int incomingCustomerId)
        {

            foreach (var claim in User.Claims)
            {
                var userServingPointId = @claim.Value;

                var waitingCustomers = await _queueRepository.GetWaitingCustomers(userServingPointId);
               
                var currentServingCustomerId = await _queueRepository.MyCurrentServingCustomer(userServingPointId);
          
                return View(new QueueVM2() { IncomingCustomerId = incomingCustomerId, 
                                             WaitingCustomers = waitingCustomers, 
                                             MyCurrentServingCustomerId = currentServingCustomerId
                                            });
            }
            return NotFound();
        }

        [Authorize(Roles = "Service Provider"), HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetNextNumber(int id) //outgoingCustomerId
        {
            foreach (var claim in User.Claims)
            {
                var serviceProviderId = @claim.Value;

                var IncomingCustomerDetails = await _queueRepository.UpdateOutGoingAndIncomingCustomerStatus(id, serviceProviderId);
                return RedirectToAction(nameof(ServicePoint), new { incomingCustomerId = IncomingCustomerDetails.Id });
            }
            return NotFound();

        }










    }
}
