using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;


namespace Queue_Management_System.Controllers
{

  


    public class QueueController : Controller
    {

          private readonly ITicketService _ticketService;

          private readonly ILogger<QueueController> _logger;

     public QueueController(ITicketService ticketService, ILogger<QueueController> logger)
    {
        _ticketService = ticketService;

        _logger = logger;
    }

        [HttpGet]
        public IActionResult CheckinPage()
        {
            return View();
        }



        // [HttpGet]
        // public IActionResult WaitingPage()
        // {
        //     return View();
        // }
     

        [Authorize, HttpGet]
        public IActionResult ServicePoint()
        {
            return View();
        }
        

             
        [HttpPost] 
        public IActionResult WaitingPage(CheckinViewModel checkinData) {
          
          Ticket newTicket = _ticketService.SaveTicketToDatabase(checkinData.ServicePointName);

          _logger.LogInformation("New ticket: {@newTicket}", newTicket.ServicePointName);

          WaitingPageViewModel waitingPageView = new WaitingPageViewModel();

          waitingPageView.TicketNumber = newTicket.TicketNumber;

          waitingPageView.ServicePointName = newTicket.ServicePointName;

          waitingPageView.IssueTime = newTicket.IssueTime;

          return View(waitingPageView);    


        } 
         
        public IActionResult DownloadTicket (WaitingPageViewModel waitingPageData) { 

          var pdfBytes = _ticketService.GenerateTicket(waitingPageData);  

          return File(pdfBytes, "application/pdf", "ticket.pdf");   

        }


    }
}
