using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Queue_Management_System.Hubs;
using Queue_Management_System.Models;

namespace Queue_Management_System.Controllers
{
    public class QueueController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly IServicePointService _servicePointService;
        private readonly IServiceProviderService _serviceProviderService;

        private readonly IHubContext<TicketHub> _hubContext;

        public QueueController(
            ITicketService ticketService,
            IServicePointService servicePointService,
            IServiceProviderService serviceProviderService,
            IHubContext<TicketHub> hubContext
            )
        {
            _ticketService = ticketService;
            _servicePointService = servicePointService;
            _serviceProviderService = serviceProviderService;
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult CheckinPage()
        {
            var viewModel = new CheckinViewModel();
            viewModel.CurrentServiceProviderServicePoints = _servicePointService.GetServicePoints();
            return View(viewModel);
        }

        [Authorize, HttpGet]
        public IActionResult ServicePoint(int currentServiceProviderId)
        {
            var viewModel = new CheckinViewModel();
            viewModel.CurrentServiceProviderId = currentServiceProviderId;
            viewModel.CurrentServiceProviderServicePoints = _serviceProviderService.GetServicePointsByServiceProviderId(currentServiceProviderId);
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult WaitingPage(CheckinViewModel checkinData)
        {
            ServicePoint servicePoint = _servicePointService.GetServicePointById(checkinData.SelectedServicePointId);
            checkinData.CurrentServicePointName = servicePoint.ServicePointName;
            Ticket newTicket = _ticketService.SaveTicketToDatabase(checkinData);
            WaitingPageViewModel waitingPageView = new WaitingPageViewModel();
            //waitingPageView.TicketNumber = newTicket.TicketId;
            //waitingPageView.ServicePointName = servicePoint.ServicePointName;
            //waitingPageView.IssueTime = newTicket.IssueTime;
            waitingPageView.Tickets = _ticketService.GetUnfinishedTickets();
            return View(waitingPageView);
        }

        public IActionResult DownloadTicket(WaitingPageViewModel waitingPageData)
        {
            var pdfBytes = _ticketService.GenerateTicket(waitingPageData);
            return File(pdfBytes, "application/pdf", "ticket.pdf");
        }

        [HttpPost, HttpGet]
        public IActionResult ServicePointDetails(int id, int serviceProviderId, int TicketCount, int CurrentTicketIndex, string direction, string successMessage)
        {
            var viewModel = new ServicePointViewModel();
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
            viewModel.SuccessMessage = successMessage;

            if (notServedTickets != null && notServedTickets.Count > 0 && CurrentTicketIndex >= 0 && CurrentTicketIndex < notServedTickets.Count)
            {
                viewModel.HasTickets = true;
                viewModel.CurrentTicketIndex = CurrentTicketIndex;
                viewModel.TicketCount = TicketCount;
                viewModel.CurrentTicketNumber = notServedTickets[CurrentTicketIndex].TicketId;
                viewModel.CurrentTicketIssueTime = notServedTickets[CurrentTicketIndex].IssueTime;
                viewModel.CurrentServicePointId = notServedTickets[CurrentTicketIndex].ServicePointId;
                viewModel.CurrentTicketStatus = notServedTickets[CurrentTicketIndex].Status;

                viewModel.ServiceStartTimeSet = _ticketService.IsServiceStartTimeSet(viewModel.CurrentTicketNumber);
            }
            else
            {
                viewModel.HasTickets = false;
            }

            return View(viewModel);
        }

        private int GetDirectionModifier(string direction)
        {
            return direction.ToLower() == "next" ? 1 : -1;
        }

        [HttpPost]
        public IActionResult CallTicket(int ticketId, int currentServicePointId, int currentServiceProviderId)
        {
            _ticketService.UpdateServiceStartTime(ticketId, DateTime.Now);

              _hubContext.Clients.All.SendAsync("ReceiveCalledTicket", ticketId, _servicePointService.GetServicePointById(currentServicePointId).ServicePointName);

            var updatedTickets = _servicePointService.findTicketsPerServicePoint(currentServicePointId);

            string successMessage = $"You started working on ticket number {ticketId}";

            var viewModel = new ServicePointViewModel
            {
                CurrentServicePointId = currentServicePointId,
                CurrentServiceProviderId = currentServiceProviderId,
                SuccessMessage = successMessage
            };

            return RedirectToAction("ServicePointDetails", new
            {
                id = viewModel.CurrentServicePointId,
                serviceProviderId = viewModel.CurrentServiceProviderId,
                successMessage = viewModel.SuccessMessage,
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkFinished(int ticketId, int currentServicePointId, string currentTicketStatus, int currentServiceProviderId)
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
                 var destinationServicePointName = _servicePointService.GetServicePointById(model.DestinationServicePointId).ServicePointName;

                if (model.DestinationServicePointId == 0)
                {
                    ModelState.AddModelError("", "You need to select a servicepoint");
                    return View("TransferTicket", new TransferTicketViewModel { TicketId = model.TicketId, OriginServicePointId = model.OriginServicePointId, AvailableServicePoints = availableServicePoints });
                }

                _ticketService.TransferTicket(model.TicketId, model.DestinationServicePointId, destinationServicePointName);
                return RedirectToAction("ServicePointDetails", new { id = model.OriginServicePointId, serviceProviderId = model.CurrentServiceProviderId });
            }

            ModelState.AddModelError("", "You need to select a servicepoint");
            return View("TransferTicket", new TransferTicketViewModel { TicketId = model.TicketId, OriginServicePointId = model.OriginServicePointId, CurrentServiceProviderId = model.CurrentServiceProviderId, AvailableServicePoints = availableServicePoints });
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
