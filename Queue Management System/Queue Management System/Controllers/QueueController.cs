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
        public async Task<IActionResult> ViewQueue()
        {
            try
            {
                string servicePointName = UserUtility.GetCurrentLoggedInUser();

                var queueEntries = await _dbOperationsRepository.GetQueueEntriesByCriteria(servicePointName);

                if (queueEntries.Count > 0)
                {
                    return View(queueEntries);
                }
                else
                {
                    return NotFound("No queue entries found for the provided service point.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching queue entries: {ex.Message}");
            }
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



       // [Authorize(Policy = "ServiceProvider")]
        [HttpGet]
        public async Task<IActionResult> ServicePoint()
        {
            string servicePointName = UserUtility.GetCurrentLoggedInUser();
            var queueEntry = await _dbOperationsRepository.GetLatestQueueEntryAsync(servicePointName);

            if (queueEntry == null)
            {
                return RedirectToAction("Error");
            }

            queue = queueEntry;

            return View(queue);
        }

        public async Task<IActionResult> ProcessSelection(int selectedServiceId, string customerName)
        {
            try
            {
                if (string.IsNullOrEmpty(customerName))
                {
                    var errorViewModel = new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };
                    return View("Error", errorViewModel);
                }
                string serviceName=await _dbOperationsRepository.GetServiceTypeNameByIdAsync(selectedServiceId)
;                ServiceTypeModel selectedService = _ticketService.GetServiceDetails(selectedServiceId);

                var queueEntry =  new QueueEntry
                {
                    TicketNumber = GenerateRandomTicketNumber(selectedService.Name),
                    ServicePoint = serviceName,
                    CustomerName = customerName,
                    CheckinTime = DateTime.Now
                };

                bool saved = await SaveQueueEntry(customerName, selectedServiceId, serviceName);
                if (!saved)
                {
                    return StatusCode(500); 
                }

                return RedirectToAction("WaitingPage");
            }
            catch (Exception ex)
            { 

                var errorViewModel = new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };
                return View("Error", errorViewModel);
            }
        }


        

        private async Task<bool> SaveQueueEntry(string CustomerName, int seerviceId, string serviceName)
        {
            return await _ticketService.CheckInAsync(GenerateRandomTicketNumber(serviceName), serviceName, CustomerName, seerviceId);
        }

        public string GenerateRandomTicketNumber(string serviceName)
        {
            Random random = new Random();
            int randomNumber = random.Next(1, 999); 

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
                string servicePointName = UserUtility.GetCurrentLoggedInUser();

                var newTicket = await _dbOperationsRepository.GetLatestQueueEntryAsync(servicePointName);

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
                            string servicePointName = UserUtility.GetCurrentLoggedInUser();

                            var newTicket = await _dbOperationsRepository.GetLatestQueueEntryAsync(servicePointName);

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
