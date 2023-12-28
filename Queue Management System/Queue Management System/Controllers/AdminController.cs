using FastReport.Table;
using FastReport;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Core.Types;
using Queue_Management_System.Models;
using Queue_Management_System.Repository;
using Queue_Management_System.ServiceInterface;
using Queue_Management_System.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FastReport.Export.PdfSimple;

namespace Queue_Management_System.Controllers
{
    // [Authorize(AuthenticationSchemes = "Bearer")]
    public class AdminController : Controller
    {

        private readonly DbOperationsRepository _dbOperationsRepository;
        private readonly string issuer = "Admin";


        public AdminController(DbOperationsRepository dbOperationsRepository)
        {
            _dbOperationsRepository = dbOperationsRepository;
        }


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



        public IActionResult ServiceProviders()
        {
            try
            {
                var serviceProviders = _dbOperationsRepository.GetServiceProviders();
                if (serviceProviders == null || !serviceProviders.Any())
                {
                    return View("ServiceProviders", new List<ServiceProviderModel>()); // Assuming your view name is "ServiceProviders"
                }

                return View("ServiceProviders", serviceProviders); // Assuming your view name is "ServiceProviders"
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
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
                    // Return a specific message or status code indicating no data
                    return NoContent(); // Or you can return a specific message like NotFound()
                }

                return Ok(queueEntriesWithNoShow);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddServiceProvider(ServiceProviderModel model)
        {
            if (ModelState.IsValid)
            {
                // Perform validation and other checks if required

                bool isServiceProviderAdded = await _dbOperationsRepository.InsertServiceProviderAsync(model);

                if (isServiceProviderAdded)
                {
                    // Redirect to the appropriate action or return a success message
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to add service provider.");
                    return View(model); // Return the view with error messages
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddService(ServiceTypeModel newService)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Perform validation and other checks if required

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
                    // Log the exception or handle it accordingly
                    ModelState.AddModelError(string.Empty, "An error occurred while adding the service type.");
                    return View(newService); // Return the view with error messages
                }
            }

            return View(newService);
        }

        [HttpGet("customers-served")]
        public async Task<ActionResult<int>> GetCustomersServedCount([FromQuery] DateTime date)
        {
            try
            {
               
                    date = DateTime.Today;
              
                int customersServedCount = await _dbOperationsRepository.GetCustomersServedCountAsync(date);
                return Ok(customersServedCount);
            }
            catch (Exception ex)
            {
                // Log the error or handle it accordingly
                return StatusCode(500, $"Error: {ex.Message}");
            }



        }

        [HttpGet("average-service-time")]
        public async Task<ActionResult<List<AverageServiceTimePerServicePoint>>> GetAverageServiceTimePerServicePoint()
        {
            try
            {
                var averageServiceTimes = await _dbOperationsRepository.GetAverageServiceTimePerServicePointAsync();
                return Ok(averageServiceTimes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
       /* public void GenerateReport()
        {
            var customersServedCount = GetCustomersServedCount();
            var averageServiceTimes = GetAverageServiceTimePerServicePoint();
            var finishedEntries = GetFinishedEntries();
            var queueEntriesWithNoShow = GetQueueEntriesWithNoShow();

            Report report = new Report();

            TextObject textObject1 = new TextObject();
            textObject1.Text = $"Customers Served: {customersServedCount}";
            // Set textObject1 properties like position, font, etc.
            report.Pages[0].ReportTitle.Objects.Add(textObject1);

            TableObject tableObject = new TableObject();
            // Populate tableObject with data from averageServiceTimes or finishedEntries or queueEntriesWithNoShow
            // Add tableObject to the report

            using (PDFSimpleExport export = new PDFSimpleExport())
            {
                export.Export(report, @"C:\Users\phill\OneDrive\Documents\Queue System\Ticket.pdf");
            }
        }*/


    }
}
