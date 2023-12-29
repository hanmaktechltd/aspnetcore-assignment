using FastReport.Table;
using FastReport;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Core.Types;
using Queue_Management_System.Models;
using Queue_Management_System.Repository;
using Queue_Management_System.ViewModels;
using FastReport.Export.PdfSimple;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Queue_Management_System.Controllers
{
    //[Authorize(Policy = "Admin")]
    public class AdminController : Controller
    {

        private readonly DbOperationsRepository _dbOperationsRepository;


        
        
        public AdminController(DbOperationsRepository dbOperationsRepository)
        
        {
            _dbOperationsRepository = dbOperationsRepository;
        }

       // [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            try
            {

                var finishedEntries = await _dbOperationsRepository.GetFinishedEntries();
                return View(finishedEntries);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while fetching data.");
                return View(new List<ServedCustomers>());
            }
        }



        public async Task<IActionResult> ServicePoints()
        {
            try
            {
                var points = await _dbOperationsRepository.GetAvailableServicesAsync();
                if (points == null || !points.Any())
                {
                    return View("ServiceProviders", new List<ServiceTypeModel>());
                }

                return View("ServicePoints", points);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching service providers.");
            }
        }



    



        [HttpGet("noShowEntries")]
        public async Task<ActionResult<List<QueueEntry>>> GetQueueEntriesWithNoShowAsync()
        {
            try
            {
                var queueEntriesWithNoShow = await _dbOperationsRepository.GetQueueEntriesWithNoShowAsync();
                if (queueEntriesWithNoShow == null || !queueEntriesWithNoShow.Any())
                {
                    return NoContent(); 
                }

                return Ok(queueEntriesWithNoShow);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddServiceProvider(ServiceProviderModel model)
        {
            if (ModelState.IsValid)
            {

                bool isServiceProviderAdded = await _dbOperationsRepository.InsertServiceProviderAsync(model);

                if (isServiceProviderAdded)
                {
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to add service provider.");
                    return View(model);
                }
            }

            return View(model);
        }
        public IActionResult ServiceProviders()
        {
            try
            {
                var serviceProviders = _dbOperationsRepository.GetServiceProviders();
                var serviceProviderViewModel = new ServiceProviderViewModel
                {
                    ServiceProviderList = serviceProviders,
                    SingleServiceProvider = serviceProviders.FirstOrDefault()
                };

                if (serviceProviders == null || !serviceProviders.Any())
                {
                    serviceProviderViewModel.ServiceProviderList = new List<ServiceProviderModel>();
                }

                return View("ServiceProviders", serviceProviderViewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching service providers.");
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddService(ServiceTypeModel newService)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Perform validation and other checks

                    bool isServiceTypeAdded = await _dbOperationsRepository.InsertServiceTypeAsync(newService);

                    if (isServiceTypeAdded)
                    {
                        return RedirectToAction("Dashboard");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Failed to add service type.");
                        return View("ServiceProviders", new List<ServiceProviderModel>());
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while adding the service type.");
                    return View(newService);
                }
            }

            return View(newService);
        }

      
       
        public async Task GenerateServiceStatisticsReportAsync()
        {
            try
            {
                List<ServiceStatistics> data = await _dbOperationsRepository.GetServiceStatistics();

                using (Report report = new Report())
                {
                    report.Load(@"C:\Users\phill\OneDrive\Documents\Queue System\DailyReport.frx");
                    report.RegisterData(data, "ServiceStatistics");
                    report.Prepare();

                    using (PDFSimpleExport export = new PDFSimpleExport())
                    {
                        export.Export(report, @"C:\Users\phill\OneDrive\Documents\Queue System\ServiceStatisticsReport.pdf");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating service statistics report: {ex.Message}");
            }
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteService(int serviceId)
        {
           try
            {
                bool updateStatus = await _dbOperationsRepository.DeleteServiceProviderById(serviceId);
                if (updateStatus)
                {
                    return Ok("Service authorized successfully.");
                }
                else
                {
                    return NotFound("Service not found or could not be authorized."); 
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting service: {ex.Message}");
            }
        }


        [HttpPut("change-service-point/{serviceId}")]
        public async Task<IActionResult> ChangeServicePoint(int serviceId, [FromBody] ServiceTypeModel changeModel)
        {
            try
            {
                

                return Ok("Service point changed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error changing service point: {ex.Message}");
            }
        }
        public async Task<IActionResult> TriggerAuthorization(int serviceId)
        {
            try
            {
                bool authorizationStatus = await AuthorizeServiceProvider(serviceId);

                if (authorizationStatus)
                {
                    TempData["AuthorizationMessage"] = "Service authorized successfully.";
                }
                else
                {
                    TempData["AuthorizationMessage"] = "Service not found or could not be authorized.";
                }

                return RedirectToAction("Dashboard"); 
            }
            catch (Exception ex)
            {
                TempData["AuthorizationMessage"] = $"Error authorizing service: {ex.Message}";
                return RedirectToAction("Dashboard"); 
            }
        }

        private async Task<bool> AuthorizeServiceProvider(int serviceId)
        {
            try
            {
                bool updateStatus = await _dbOperationsRepository.UpdateServiceProviderAuthorization(serviceId);
                return updateStatus; // Return the status of the update
            }
            catch (Exception ex)
            {
                throw new Exception($"Error authorizing service: {ex.Message}");
            }
        }
    }
}
