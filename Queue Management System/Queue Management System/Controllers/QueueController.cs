using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;


namespace Queue_Management_System.Controllers
{


    public class QueueController : Controller
    {

          private readonly ITicketService _ticketService;


          private readonly IServicePointService _servicePointService;

          private readonly ILogger<QueueController> _logger;

     public QueueController(ITicketService ticketService, IServicePointService servicePointService, ILogger<QueueController> logger)
    {
        _ticketService = ticketService;

        _servicePointService = servicePointService;

        _logger = logger;
    }

        [HttpGet]
        public IActionResult CheckinPage()
        {

         var viewModel = new CheckinViewModel();
         
         List <ServicePoint> servicePoints = _servicePointService.GetServicePoints();


          viewModel.ServicePoints = servicePoints;

          return View(viewModel);
        }

        // [HttpGet]
        // public IActionResult WaitingPage()
        // {
        //     return View();
        // }
     

        [Authorize, HttpGet]
        public IActionResult ServicePoint()
        {    

          var viewModel = new CheckinViewModel();
         
         List <ServicePoint> servicePoints = _servicePointService.GetServicePoints();

          viewModel.ServicePoints = servicePoints;
        

            return View(viewModel);
        }
        

             
        [HttpPost] 
        public IActionResult WaitingPage(CheckinViewModel checkinData) {
          
          Ticket newTicket = _ticketService.SaveTicketToDatabase(checkinData);

          _logger.LogInformation("checkindata selected id: {@checkinData}", checkinData.SelectedServicePointId);
          _logger.LogInformation("New ticket id: {@newTicket}", newTicket.TicketId);

          WaitingPageViewModel waitingPageView = new WaitingPageViewModel();

          waitingPageView.TicketNumber = newTicket.TicketId;

          ServicePoint servicePoint = _servicePointService.GetServicePointById(checkinData.SelectedServicePointId);

          waitingPageView.ServicePointName = servicePoint.ServicePointName;

          waitingPageView.IssueTime = newTicket.IssueTime;

          return View(waitingPageView);    


        } 
         
        public IActionResult DownloadTicket (WaitingPageViewModel waitingPageData) { 

          var pdfBytes = _ticketService.GenerateTicket(waitingPageData);  

          return File(pdfBytes, "application/pdf", "ticket.pdf");   

        }

          
        [HttpPost]
        [HttpGet]
        public IActionResult ServicePointDetails (int id, int TicketCount, int CurrentTicketIndex, string direction) {
        
        ServicePointViewModel viewModel = new ServicePointViewModel();

        List <Ticket> fetchedTickets = _servicePointService.findTicketsPerServicePoint(id);

        viewModel.AllTickets = fetchedTickets;

        List<Ticket> notServedTickets = fetchedTickets.Where(t => t.Status == "Not Served").ToList();

        viewModel.NotServedTickets = notServedTickets;

        TicketCount = notServedTickets.Count;
        
        //Ticket currentTicket = _servicePointService.GetCurrentTicketPerServicePoint(checkInData.SelectedServicePointId);
        //Ticket nextTicket = _servicePointService.GetNextTicketPerServicePoint(checkInData.SelectedServicePointId); 

        if (!string.IsNullOrEmpty(direction))
        {
            CurrentTicketIndex += GetDirectionModifier(direction);
        }

      
        viewModel.CurrentServicePointId = id;
        

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
          public IActionResult MarkFinished(int ticketId, int currentServicePointId)
          {
              _ticketService.MarkAsFinished(ticketId);
              return RedirectToAction("ServicePointDetails", new { id = currentServicePointId });
          }

         [HttpPost]
         [ValidateAntiForgeryToken]

          public IActionResult MarkNoShow(int ticketId, int currentServicePointId)
          {
               _ticketService.MarkAsNoShow(ticketId);
              return RedirectToAction("ServicePointDetails", new { id = currentServicePointId });
          }

          public IActionResult TransferTicket(int ticketId, int currentServicePointId)
          {
              
             Console.WriteLine("currentServicePoinId is {0}", currentServicePointId);
             List <ServicePoint> availableServicePoints = _servicePointService.GetServicePoints();
              
              return View("TransferTicket", new TransferTicketViewModel { TicketId = ticketId, OriginServicePointId = currentServicePointId, AvailableServicePoints = availableServicePoints });
          }

          [HttpPost]
          public IActionResult CompleteTransfer(TransferTicketViewModel model)
          {

               Console.WriteLine("OriginServicePointId is : {0}", model.OriginServicePointId);
                 Console.WriteLine("DestinationServicePointId is : {0}", model.DestinationServicePointId);
                 Console.WriteLine("TicketId is : {0}", model.TicketId);
  
              _ticketService.TransferTicket(model.TicketId, model.DestinationServicePointId);
    
              return RedirectToAction("ServicePointDetails", new { currentServicePointId = model.OriginServicePointId });
          }



         [HttpPost]
         [ValidateAntiForgeryToken]
          public IActionResult RecallTicket(int ticketId, int currentServicePointId)
          {

            Console.WriteLine("ticketId is {0}", ticketId);
            Console.WriteLine("currentServicePointId is {0}", currentServicePointId);
              _ticketService.UpdateTicketStatus(ticketId, "Not Served");
              return RedirectToAction("ServicePointDetails", new { id = currentServicePointId });
          }



    }
}
