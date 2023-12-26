using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;
using Queue_Management_System.Repository;
using Queue_Management_System.ServiceInterface;
using System.Diagnostics;

namespace Queue_Management_System.Controllers
{
    public class QueueController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly DbOperationsRepository _dbOperationsRepository;
        private readonly ServiceProviderModel serviceProvider;
        private readonly IServicePointOperations _servicePointOperations;
        QueueEntry queue = new QueueEntry();
        int recallCount= 0;
        

        public QueueController(ITicketService ticketService, DbOperationsRepository dbOperationsRepository, IServicePointOperations servicePointOperations)
        {
            _ticketService = ticketService;
            _dbOperationsRepository = dbOperationsRepository;
            _servicePointOperations = servicePointOperations;
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
            var queueEntry = await _dbOperationsRepository.GetLatestQueueEntryAsync(2);

            if (queueEntry == null)
            {
                return RedirectToAction("Error");
            }

            queue = queueEntry;

            return View(queue);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessSelection(int selectedServiceId, string customerName)
        {
            if (string.IsNullOrEmpty(customerName))
            {
                var errorViewModel = new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };
                return View("Error", errorViewModel);
            }

            ServiceTypeModel selectedService = _ticketService.GetServiceDetails(selectedServiceId); // Get service details

            // Create a QueueEntry object with the selected service and customer details
            var queueEntry = CreateQueueEntry(selectedService, customerName);

            // Save the queue entry to the database
            bool saved = await SaveQueueEntry(queueEntry, selectedServiceId);
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
                TicketNumber = GenerateRandomTicketNumber(selectedService.Name), // Generate a random ticket number
                ServicePoint = selectedService.Name, // Use service name as the service point or adjust as needed
                CustomerName = customerName,
                CheckinTime = DateTime.Now // Timestamp for check-in time
            };
        }

        private async Task<bool> SaveQueueEntry(QueueEntry queueEntry, int seerviceId)
        {
            return await _ticketService.CheckInAsync(queueEntry.TicketNumber, queueEntry.ServicePoint, queueEntry.CustomerName, seerviceId);
        }

        public string GenerateRandomTicketNumber(string serviceName)
        {
            Random random = new Random();
            int randomNumber = random.Next(1, 999); // Generate a random 4-digit number

            // Construct a ticket number using service details and the random number
            string ticketNumber = $"{serviceName.Substring(0, 3)}-{randomNumber}";

            return ticketNumber;
        }

        [HttpPost]
        public async Task<IActionResult> MarkFinished(string ticketNumber)
        {
            //check the providerid
            bool success = await _servicePointOperations.MarkFinishedAndInsert(ticketNumber, 1);

            if (success)
            {
                var newTicket = await _dbOperationsRepository.GetLatestQueueEntryAsync(1);

                if (newTicket != null)
                {
                    return PartialView("_TicketDetailsPartial", newTicket);
                }
            }

            return Content("No ticket details available.");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateRecallCount(string ticketNumber)
        {
            int currentRecallCount = await _dbOperationsRepository.GetRecallCountAsync(ticketNumber);

            while (currentRecallCount < 3)
            {
                currentRecallCount++;
                bool updated = await _servicePointOperations.UpdateRecallCount(ticketNumber, currentRecallCount);

                if (updated)
                {
                    if (currentRecallCount ==2)
                    {
                        bool noShow = await _servicePointOperations.MarkAsNoShow(ticketNumber);
                       if (noShow)
                        {
                            var newTicket = await _dbOperationsRepository.GetLatestQueueEntryAsync(1);

                            if (newTicket != null)
                            {
                                return PartialView("_TicketDetailsPartial", newTicket);
                            }
                        }
                    }

                    
                }
                else
                {
                    return BadRequest("Failed to update recall count.");
                }
            }

            return BadRequest("Recall count cannot be updated as it has reached the limit.");
        }







        private ServiceProviderModel GetUserFromLoggedInUser()
        {
            // Get the user's identity from HttpContext
            var userIdentity = HttpContext.User.Identity;

            // Example: Assuming the user is authenticated and a ServiceProviderModel is stored in claims
            if (userIdentity.IsAuthenticated)
            {
                // Accessing claims to retrieve ServiceProviderModel information
                var serviceProviderIdClaim = HttpContext.User.FindFirst("ServiceProviderId");
                var serviceProviderUsernameClaim = HttpContext.User.FindFirst("ServiceProviderUsername");

                // Check if both claims are present
                if (serviceProviderIdClaim != null && serviceProviderUsernameClaim != null)
                {
                    // Parse claims to build the ServiceProviderModel
                    if (int.TryParse(serviceProviderIdClaim.Value, out int serviceProviderId))
                    {
                        var serviceProvider = new ServiceProviderModel
                        {
                            Id = serviceProviderId,
                            Username = serviceProviderUsernameClaim.Value,
                            // Add other properties from claims as needed
                        };

                        return serviceProvider;
                    }
                }
            }

            return null; // Return null if the user is not authenticated or if claims are not as expected
        }

        [HttpGet]
        public async Task<ActionResult<List<QueueEntry>>> GetQueueEntries(int servicePointId)
        {
            try
            {
                var queueEntries = await _dbOperationsRepository.GetQueueEntriesByCriteria(1);

                if (queueEntries == null || queueEntries.Count == 0)
                {
                    return NotFound("No queue entries found for the specified criteria.");
                }

                return PartialView("_ViewQueue", queueEntries);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching queue entries: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching queue entries.");
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<(string ServicePoint, int ServiceTypeId)>>> GetServiceProviders()
        {
            try
            {
                var serviceProviderDetails = await _dbOperationsRepository.GetServiceProviderDetailsAsync();

                if (serviceProviderDetails == null || serviceProviderDetails.Count == 0)
                {
                    return NotFound("No service provider details found.");
                }

                return PartialView("_TransferNumberPartial", serviceProviderDetails);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching service provider details: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching service provider details.");
            }
        }

        [HttpPost("transfer-ticket")]
        public async Task<ActionResult<bool>> TransferTicketToService(string ticketNumber, string servicePoint, int servicePointId)
        {
            var success = await _dbOperationsRepository.TransferTicketToService(ticketNumber, servicePoint, servicePointId);

            if (success)
            {
                return Ok(true); // Successfully updated the ticket's service point
            }
            else
            {
                return StatusCode(500, "Failed to transfer ticket to the service.");
            }
        }


    }

}
