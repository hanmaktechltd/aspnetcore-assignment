using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;
using Queue_Management_System.Repository;
using Queue_Management_System.ServiceInterface;

namespace Queue_Management_System.Controllers
{
    public class QueueController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly DbOperationsRepository _dbOperationsRepository;

        public QueueController(ITicketService ticketService, DbOperationsRepository dbOperationsRepository)
        {
            _ticketService = ticketService;
            _dbOperationsRepository = dbOperationsRepository;
        }

        [HttpGet]
        public async Task<IActionResult> CheckinPage()
        {
            var services = await _ticketService.GetAvailableServicesAsync();
            return View(services);
        }


        [HttpGet]
        public async Task<IActionResult> WaitingPage()
        {
            var topQueueEntry = await _dbOperationsRepository.GetTopQueueEntryAsync();

            if (topQueueEntry != null)
            {
                var waitingPageModel = new WaitingPageModel(
                    topQueueEntry.TicketNumber,
                    topQueueEntry.ServicePoint,
                    topQueueEntry.CustomerName,
                    topQueueEntry.CheckinTime
                );

                return View(waitingPageModel);
            }

            return View(new WaitingPageModel("", "", "", DateTime.MinValue));
        }




       
       
       // [Authorize(AuthenticationSchemes = "ServiceUser")]
        [HttpGet]
        public async Task<IActionResult> ServicePoint()
        {
            var queueEntry = await _dbOperationsRepository.GetLatestQueueEntryAsync();

            if (queueEntry == null)
            {
                // Handle the case where no queue entry is found
                // You might redirect to an error page or perform other actions
                return RedirectToAction("Error");
            }
            var queue = new QueueEntry();
            queue.TicketNumber=queueEntry.TicketNumber;

            return View(queue);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessSelection(int selectedServiceId, string customerName)
        {
            ServiceTypeModel selectedService = _ticketService.GetServiceDetails(selectedServiceId); // Get service details

            // Create a QueueEntry object with the selected service and customer details
            var queueEntry = CreateQueueEntry(selectedService, customerName);

            // Save the queue entry to the database
            bool saved = await SaveQueueEntry(queueEntry);
            if (!saved)
            {
                // Handle error if the queue entry couldn't be saved
                // Optionally, display an error message or take corrective actions
            }

            // Generate ticket and perform other necessary actions
            // byte[] ticketBytes = _ticketService.GenerateTicket(selectedService); // Generate ticket
            // string ticketFilePath = _ticketService.SaveTicketToFile(ticketBytes); // Save ticket to file

            // Redirect to waiting page or another action with necessary data
            // return RedirectToAction("Waiting", new { serviceId = selectedServiceId, ticketPath = ticketFilePath });
            return RedirectToAction("WaitingPage", new { serviceId = selectedServiceId });

        }

        private QueueEntry CreateQueueEntry(ServiceTypeModel selectedService, string customerName)
        {
            return new QueueEntry
            {
                TicketNumber = GenerateRandomTicketNumber(), // Generate a random ticket number
                ServicePoint = selectedService.Name, // Use service name as the service point or adjust as needed
                CustomerName = customerName,
                CheckinTime = DateTime.Now // Timestamp for check-in time
            };
        }

        private async Task<bool> SaveQueueEntry(QueueEntry queueEntry)
        {
            return await _ticketService.CheckInAsync(queueEntry.TicketNumber, queueEntry.ServicePoint, queueEntry.CustomerName);
        }

        private string GenerateRandomTicketNumber()
        {
            // Generate a random ticket number starting with 'Q' followed by random digits
            Random random = new Random();
            string ticketNumber = "Q" + random.Next(1, 99).ToString(); // Adjust range as needed
            return ticketNumber;
        }


    }

}
