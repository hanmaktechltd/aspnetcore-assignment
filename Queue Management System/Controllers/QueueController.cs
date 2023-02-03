﻿using Microsoft.AspNetCore.Authorization;
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
        public IActionResult WaitingPage()
        {
            return View();
        }

        [Authorize(Roles = "Service Provider"), HttpGet]
        public async Task<ActionResult<IEnumerable<QueueVM>>> ServicePoint()
        {

            foreach (var claim in User.Claims)
            {
                var userServingPointId = @claim.Value;

                var waitingCustomers = await _queueRepository.GetWaitingCustomers(userServingPointId);
                return View(waitingCustomers);
            }
            return NotFound();
        }

        [Authorize(Roles = "Service Provider"), HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetNextNumber(int id)
        {
            foreach (var claim in User.Claims)
            {
                var userServingPointId = @claim.Value;

                var ServiceProviderDetails = await _queueRepository.UpdateStatus(id, userServingPointId);
                return RedirectToAction(nameof(ServicePoint), new { id = ServiceProviderDetails.Id });
            }
            return NotFound();

        }










    }
}
