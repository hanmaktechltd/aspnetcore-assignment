using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult ServicePoint()
        {
            return View();
        }


    }
}
