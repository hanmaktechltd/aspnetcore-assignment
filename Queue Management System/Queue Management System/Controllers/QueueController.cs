using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Queue_Management_System.Repositories;
using Queue_Management_System.Models;
using Queue_Management_System.Services;
using FastReport.Web;

namespace Queue_Management_System.Controllers
{
    public class QueueController : Controller
    {
        private readonly IServiceRepository _serviceRepository;
        
        private readonly ITicketService _ticketService;

        private readonly IReportService _reportService;

        public QueueController(IServiceRepository serviceRepository, ITicketService ticketService, IReportService reportService)
        {
            _serviceRepository = serviceRepository;
            _ticketService = ticketService;
            _reportService = reportService;
        }

        [HttpGet]
        public IActionResult CheckinPage(string serviceId)
        {

            if (serviceId != null)
            {
                TicketModel ticket = _ticketService.CreateTicket(serviceId);
                //_ticketRepository.save ticket

                WebReport report = _reportService.GenerateTicketReport(ticket.TicketNumber, ticket.ServiceId, ticket.TimePrinted);
                //redirect to waiting page with report attached
                ViewBag.WebReport = report;
                return View("WaitingPage");
            }

            var services = _serviceRepository.GetServices();
            var servicesViewModel = new ServicesViewModel(){
                Services = services.ToList().ToArray() // refactor
            };

            return View(servicesViewModel);
        }



        [HttpGet]
        public IActionResult WaitingPage()
        {
            return View();
        }



        [Authorize, HttpGet]
        public IActionResult ServicePoint(string buttonName, string serviceId)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("servicePointId")))
            {
                //logged in user has no service point
                //return no configured service point view
            }


            if (buttonName == "GetNextNumber")
            {
                //_ticket repo.get nexno
                //if number, setviewdatamessage, setviewdatacalled = true
            }

            if (buttonName == "RecallNextNoShowNumber")
            {
                //get number from no show tickets, remove from no show
                //set viewdatacalledmessage, set viewdatacallednumber to true
            }

            if (buttonName == "MarkAsShow")
            {
                //mark ticket as shown in db
                //record time marked as shown in db
                //set viewdatacalled = true, set viewdatashowedup = true

            }

            if (buttonName == "MarkAsNoShow")
            {
                //set showed up to false in db
                //transfer ticket to no show tickets
                //
            }

            if (buttonName == "MarkAsFinished")
            {
                //mark ticket as finished in db and record finsihed time
                //set viewdatacalled = true, set viewdatashowedup = true, set viewdatafished = true
                //pass services to view excluding current service
            
            }

            if (serviceId != null)
            {
                TicketModel ticket = _ticketService.TransferTicket(ticketNumber, serviceId);
                //ticketrepo.saveticket

                WebReport report = _reportService.GenerateTicketReport(ticket.TicketNumber, ticket.ServiceId, ticket.TimePrinted);

                ViewBag.WebReport = report;
            }

            //configure view model
            var servicePointId = HttpContext.Session.GetString("servicePointId");
            var serviceDescription = HttpContext.Session.GetString("serviceDescription");

            var ticketsInQueue = _ticketRepository.GetUnservedTicketsByServiceID(servicePointId); //get service id from session
            var noShowTickets = null; //get from list of noshow tickets in session
            var services = _serviceRepository.GetServices(); //todo only required after markasfinshed

            var viewModel = new ServicePointViewModel(){
                ServicePointId = servicePointId, //get from session
                ServiceDescription = serviceDescription, //get from session
                TicketsInQueue = ticketsInQueue,
                NoshowTickets = noShowTickets,
                Services = services,
            };
            return View(viewModel);
        }

//todo log out user and clear sesion

    }
}
