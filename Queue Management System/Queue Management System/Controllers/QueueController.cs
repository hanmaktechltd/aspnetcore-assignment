using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;

namespace Queue_Management_System.Controllers
{
    public class QueueController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly IServicePointService _servicePointService;
        private readonly IServiceProviderService _serviceProviderService;
        private readonly ILogger<QueueController> _logger;

        public QueueController(
            ITicketService ticketService,
            IServicePointService servicePointService,
            IServiceProviderService serviceProviderService,
            ILogger<QueueController> logger)
        {
            _ticketService = ticketService;
            _servicePointService = servicePointService;
            _serviceProviderService = serviceProviderService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult CheckinPage()
        {
            var viewModel = new CheckinViewModel();
            List<ServicePoint> servicePoints = _servicePointService.GetServicePoints();
            viewModel.CurrentServiceProviderServicePoints = servicePoints;
            return View(viewModel);
        }

        [Authorize, HttpGet]
        public IActionResult ServicePoint(int currentServiceProviderId)
        {
            var viewModel = new CheckinViewModel();
            viewModel.CurrentServiceProviderId = currentServiceProviderId;
            var servicePoints = _serviceProviderService.GetServicePointsByServiceProviderId(currentServiceProviderId);
            _logger.LogInformation("ServicePoints for ServiceProviderId {ServiceProviderId}: {@ServicePoints}", currentServiceProviderId, servicePoints);
            viewModel.CurrentServiceProviderServicePoints = servicePoints;
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult WaitingPage(CheckinViewModel checkinData)
        {
            ServicePoint servicePoint = _servicePointService.GetServicePointById(checkinData.SelectedServicePointId);
            checkinData.CurrentServicePointName = servicePoint.ServicePointName;
            Ticket newTicket = _ticketService.SaveTicketToDatabase(checkinData);
            WaitingPageViewModel waitingPageView = new WaitingPageViewModel();
            waitingPageView.TicketNumber = newTicket.TicketId;
            waitingPageView.ServicePointName = servicePoint.ServicePointName;
            waitingPageView.IssueTime = newTicket.IssueTime;
            return View(waitingPageView);
        }

        public IActionResult DownloadTicket(WaitingPageViewModel waitingPageData)
        {
            var pdfBytes = _ticketService.GenerateTicket(waitingPageData);
            return File(pdfBytes, "application/pdf", "ticket.pdf");
        }

        [HttpPost, HttpGet]
        public IActionResult ServicePointDetails(int id, int serviceProviderId, int TicketCount, int CurrentTicketIndex, string direction)
        {

            ServicePointViewModel viewModel = new ServicePointViewModel();
            List<Ticket> fetchedTickets = _servicePointService.findTicketsPerServicePoint(id);
            viewModel.AllTickets = fetchedTickets;
            List<Ticket> notServedTickets = fetchedTickets.FindAll(t => t.Status == "Not Served");
            viewModel.NotServedTickets = notServedTickets;
            TicketCount = notServedTickets.Count;

            if (!string.IsNullOrEmpty(direction))
            {
                CurrentTicketIndex += GetDirectionModifier(direction);
            }

            viewModel.CurrentServicePointId = id;
            viewModel.CurrentServiceProviderId = serviceProviderId;

            if (notServedTickets != null && notServedTickets.Count > 0 && CurrentTicketIndex >= 0 && CurrentTicketIndex < notServedTickets.Count)
            {
                viewModel.HasTickets = true;
                viewModel.CurrentTicketIndex = CurrentTicketIndex;
                viewModel.TicketCount = TicketCount;
                viewModel.CurrentTicketNumber = notServedTickets[CurrentTicketIndex].TicketId;
                viewModel.CurrentTicketIssueTime = notServedTickets[CurrentTicketIndex].IssueTime;
                viewModel.CurrentServicePointId = notServedTickets[CurrentTicketIndex].ServicePointId;
            }
            else
            {
                viewModel.HasTickets = false;
            }

            return View(viewModel);
        }

        private int GetDirectionModifier(string direction)
        {
            int modifier = direction.ToLower() == "next" ? 1 : -1;
            Console.WriteLine(modifier);
            return modifier;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkFinished(int ticketId, int currentServicePointId, int currentServiceProviderId)
        {
            var serviceProviderUsername = _serviceProviderService.GetServiceProviderById(currentServiceProviderId).Username;
            _ticketService.MarkAsFinished(ticketId);
            _ticketService.SetServiceProviderForTicket(ticketId, serviceProviderUsername);
            return RedirectToAction("ServicePointDetails", new { id = currentServicePointId, serviceProviderId = currentServiceProviderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkNoShow(int ticketId, int currentServicePointId, int currentServiceProviderId)
        {
            _ticketService.MarkAsNoShow(ticketId);
            return RedirectToAction("ServicePointDetails", new { id = currentServicePointId, serviceProviderId = currentServiceProviderId });
        }

        public IActionResult TransferTicket(int ticketId, int currentServicePointId, int currentServiceProviderId)
        {
            if (ModelState.IsValid)
            {
                List<ServicePoint> availableServicePoints = _servicePointService.GetServicePoints();
                return View("TransferTicket", new TransferTicketViewModel { TicketId = ticketId, OriginServicePointId = currentServicePointId, CurrentServiceProviderId = currentServiceProviderId, AvailableServicePoints = availableServicePoints });
            }

            return RedirectToAction("ServicePointDetails", new { id = currentServicePointId, serviceProviderId = currentServiceProviderId });
        }

        [HttpPost]
        public IActionResult CompleteTransfer(TransferTicketViewModel model)
        {
            List<ServicePoint> availableServicePoints = _servicePointService.GetServicePoints();

            if (ModelState.IsValid)
            {
                if (model.DestinationServicePointId == 0)
                {
                    ModelState.AddModelError("", "You need to select a servicepoint");
                    return View("TransferTicket", new TransferTicketViewModel { TicketId = model.TicketId, OriginServicePointId = model.OriginServicePointId, AvailableServicePoints = availableServicePoints });
                }

                _ticketService.TransferTicket(model.TicketId, model.DestinationServicePointId);
                return RedirectToAction("ServicePointDetails", new { id = model.OriginServicePointId, serviceProviderId = model.CurrentServiceProviderId });
            }

            ModelState.AddModelError("", "You need to select a servicepoint");
            return View("TransferTicket", new TransferTicketViewModel { TicketId = model.TicketId, OriginServicePointId = model.OriginServicePointId, AvailableServicePoints = availableServicePoints });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RecallTicket(int ticketId, int currentServicePointId, int currentServiceProviderId)
        {
            _ticketService.UpdateTicketStatus(ticketId, "Not Served");
            return RedirectToAction("ServicePointDetails", new { id = currentServicePointId, serviceProviderId = currentServiceProviderId });
        }
    }
}
