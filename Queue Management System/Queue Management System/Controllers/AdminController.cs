using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Queue_Management_System.Models;
using Queue_Management_System.Services;
using System.Security.Claims;

namespace Queue_Management_System.Controllers
{
    public class AdminController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IAdminRepository _adminRepository;
        private NpgsqlConnection _connection;

        public AdminController(IConfiguration configuration, IWebHostEnvironment hostEnvironment, IAdminRepository adminRepository)
        {
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
            _adminRepository = adminRepository;
        }

        private void OpenConnection()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            _connection = new NpgsqlConnection(connectionString);
            _connection.Open();
        }

        [Authorize(AuthenticationSchemes = "AdminAuthentication")]
        public IActionResult Authenticated()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync(string EmailAddress, string Password)
        {
            if (ModelState.IsValid)
            {
                var admin = await _adminRepository.Login(EmailAddress, Password);
                if (admin is not null)
                {
                    TempData["success"] = "Login Successfully";
                    return RedirectToAction("Authenticated");
                }
                TempData["error"] = "Invalid Login Credentials";
            }
            return View();
        }

        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync("AdminAuthentication");
            TempData["success"] = "Logout Successfull";
            return RedirectToAction("Login");
        }

        // All patients on the Queue
        public async Task<IActionResult> MainQueue()
        {
            var mainQueue = await _adminRepository.MainQueue();
            if (mainQueue is null)
            {
                TempData["error"] = "No customers currently on the queue";
                return RedirectToAction("Authenticated");
            }
            return View(mainQueue);
        }

        // Fetching all service points and passing them down to the view
        [Authorize(AuthenticationSchemes = "AdminAuthentication")]
        public async Task<IActionResult> ServicePointsAsync()
        {
            var servicePoints = await _adminRepository.GetAllServicePoints();
            return View(servicePoints);
        }

        // Fetching all service providers and passing them down to the view
        [Authorize(AuthenticationSchemes = "AdminAuthentication")]
        public async Task<IActionResult> ServiceProvidersAsync()
        {
            var serviceProviders = await _adminRepository.GetAllServiceProviders();
            return View(serviceProviders);
        }

        [Authorize(AuthenticationSchemes = "AdminAuthentication")]
        // AddServiceProvider View
        public IActionResult AddServiceProvider()
        {
            return View();
        }

        [HttpGet, Authorize(AuthenticationSchemes = "AdminAuthentication")]
        // AddServicePoint View
        public IActionResult AddServicePoint()
        {
            return View();
        }

        // Saving the new service point to the db
        [HttpPost]
        public async Task<IActionResult> AddServicePoint(ServicePoint servicePoint)
        {
            await _adminRepository.CreateServicePoint(servicePoint);
            TempData["success"] = "Service Point Added Successfully";
            return RedirectToAction("ServicePoints");
        }

        [Authorize(AuthenticationSchemes = "AdminAuthentication")]
        public async Task<IActionResult> EditServicePoint(int Id)
        {
            var servicePoint = await _adminRepository.GetServicePointById(Id);
            if (servicePoint == null)
            {
                return NotFound();
            }
            return View(servicePoint);
        }

        [HttpPost]
        public async Task<IActionResult> EditServicePoint(ServicePoint servicePoint)
        {
            await _adminRepository.UpdateServicePoint(servicePoint);
            TempData["success"] = "Service Point Edited Successfully";
            return RedirectToAction("ServicePoints");
        }

        [Authorize(AuthenticationSchemes = "AdminAuthentication")]
        public async Task<IActionResult> DeleteServicePoint(int id)
        {
            var servicePoint = await _adminRepository.GetServicePointById(id);
            if (servicePoint == null)
            {
                TempData["error"] = "An error occurred please try again later";

                return RedirectToAction("ServiceProviders");
            }
            return View(servicePoint);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "AdminAuthentication")]
        public async Task<IActionResult> DeleteServicePoint(ServicePoint servicePoint)
        {
            try
            {
                await _adminRepository.DeleteServicePoint(servicePoint);
                TempData["success"] = "Service Point Deleted Successfully";
            }
            catch (PostgresException ex) when (ex.SqlState == "23503")
            {
                TempData["error"] = "Cannot delete this ServicePoint since there are Service Providers Linked to it";
            }
            catch (Exception)
            {
                TempData["error"] = "An error occurred, please try again later";
            }

            return RedirectToAction("ServicePoints");
        }

        [Authorize(AuthenticationSchemes = "AdminAuthentication")]
        [HttpPost]
        public async Task<IActionResult> AddServiceProvider(Models.ServiceProvider serviceProvider)
        {
            await _adminRepository.CreateServiceProvider(serviceProvider);
            TempData["success"] = "Service Provider Added Successfully";
            return RedirectToAction("ServiceProviders");
        }

        [Authorize(AuthenticationSchemes = "AdminAuthentication")]
        public async Task<IActionResult> EditServiceProvider(int id)
        {
            var serviceProvider = await _adminRepository.GetServiceProviderById(id);
            if (serviceProvider == null)
            {
                TempData["error"] = "An error occurred please try again later";

                return RedirectToAction("ServiceProviders");
            }
            return View(serviceProvider);
        }

        [HttpPost]
        public async Task<IActionResult> EditServiceProvider(Models.ServiceProvider serviceProvider)
        {
            await _adminRepository.UpdateServiceProvider(serviceProvider);
            TempData["success"] = "Service Provider Modified Successfully";
            return RedirectToAction("ServiceProviders");

        }

        [Authorize(AuthenticationSchemes = "AdminAuthentication")]
        public async Task<IActionResult> DeleteServiceProvider(int id)
        {
            var serviceProvider = await _adminRepository.GetServiceProviderById(id);
            if (serviceProvider == null)
            {
                TempData["error"] = "An error occurred please try again later";

                return RedirectToAction("ServiceProviders");
            }
            return View(serviceProvider);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "AdminAuthentication")]
        public async Task<IActionResult> DeleteServiceProviderAsync(Models.ServiceProvider serviceProvider)
        {
            try
            {
                await _adminRepository.DeleteServiceProvider(serviceProvider);
                TempData["success"] = "Service Provider Deleted Successfully";
            }
            catch (PostgresException ex) when (ex.SqlState == "23503")
            {
                TempData["error"] = "Cannot delete this service provider as he has already served customers";
            }
            catch (Exception)
            {
                TempData["error"] = "An error occurred, please try again later";
            }

            return RedirectToAction("ServiceProviders");
        }

        [Authorize(AuthenticationSchemes = "AdminAuthentication")]
        public IActionResult AnalyticalReports()
        {
            return View();
        }

        [Authorize(AuthenticationSchemes = "AdminAuthentication")]
        public async Task<IActionResult> CustomersServedAsync([FromServices] IWebHostEnvironment webHostEnvironment)
        {
            // Load the report file
            string templatePath = $"Reports/CustomersServed.frx";
            string reportPath = Path.Combine(webHostEnvironment.ContentRootPath, templatePath);
            using (Report report = new Report())
            {
                report.Load(reportPath);


                // Set up the data connection
                OpenConnection();
                var sql = "SELECT sp.\"Name\" AS ServiceProviderName, COUNT(DISTINCT ct.\"Id\") AS NumCustomersServed " +
                    "FROM public.\"Customers\" ct " +
                    "JOIN public.\"ServiceProviders\" sp ON ct.\"ServiceProviderId\" = sp.\"Id\" " +
                    "WHERE ct.\"Status\" = 'Finished' " +
                    "GROUP BY sp.\"Name\"";

                using (NpgsqlCommand command = new NpgsqlCommand(sql, _connection))
                {
                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var serviceProviderName = reader.GetString(0);
                            var numCustomersServed = reader.GetInt64(1);
                            report.SetParameterValue($"num_customers_{serviceProviderName}", numCustomersServed);
                        }
                        reader.Close();
                    }
                }

                using (var stream = new MemoryStream())
                using (var export = new PDFSimpleExport())
                {
                    // Prepare the report data
                    report.Prepare();
                    export.Export(report, stream);
                    // Set the position of the MemoryStream back to the beginning
                    stream.Seek(0, SeekOrigin.Begin);
                    // Create a new MemoryStream and copy the contents of the original stream to it
                    var outputStream = new MemoryStream(stream.ToArray());

                    return new FileStreamResult(outputStream, "application/pdf");
                }
            }
        }

        [Authorize(AuthenticationSchemes = "AdminAuthentication")]
        public async Task<IActionResult> AvgWaitingTimeAsync([FromServices] IWebHostEnvironment webHostEnvironment)
        {
            // Load the report file
            string templatePath = $"Reports/AverageWaiting.frx";
            string reportPath = Path.Combine(webHostEnvironment.ContentRootPath, templatePath);
            using (Report report = new Report())
            {
                report.Load(reportPath);

                OpenConnection();
                var sql = "SELECT AVG(EXTRACT(EPOCH FROM (\"Customers\".\"CallTime\" - \"Customers\".\"CheckInTime\"))) / 60 as AvgWaitingTime, AVG(EXTRACT(EPOCH FROM (CASE WHEN \"Customers\".\"EndServiceTime\" >= \"Customers\".\"StartServiceTime\" THEN (\"Customers\".\"EndServiceTime\" - \"Customers\".\"StartServiceTime\") ELSE NULL END ))) / 60 AvgServiceTime, \"ServiceProviders\".\"Name\" as ServiceProviderName FROM public.\"Customers\" JOIN public.\"ServiceProviders\" ON \"Customers\".\"ServiceProviderId\" = \"ServiceProviders\".\"Id\" WHERE \"Customers\".\"Status\" = 'Finished' GROUP BY \"ServiceProviders\".\"Name\";";

                using (NpgsqlCommand command = new NpgsqlCommand(sql, _connection))
                {
                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var serviceProviderName = reader.GetString(2);
                            var avgWaitingTime = Math.Round(reader.GetDouble(0));
                            var avgServiceTime = Math.Round(reader.GetDouble(1));

                            report.SetParameterValue($"avg_waiting_time_{serviceProviderName}", avgWaitingTime);
                            report.SetParameterValue($"avg_service_time_{serviceProviderName}", avgServiceTime);
                        }
                    }
                }

                using (var stream = new MemoryStream())
                using (var export = new PDFSimpleExport())
                {
                    // Prepare the report data
                    report.Prepare();
                    export.Export(report, stream);
                    // Set the position of the MemoryStream back to the beginning
                    stream.Seek(0, SeekOrigin.Begin);
                    // Create a new MemoryStream and copy the contents of the original stream to it
                    var outputStream = new MemoryStream(stream.ToArray());

                    return new FileStreamResult(outputStream, "application/pdf");
                }
            }
        }
    }
}