using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Queue_Management_System.Models;
using Queue_Management_System.Services;
using Queue_Management_System.Models.Data;
using System.Security.Claims;

namespace Queue_Management_System.Controllers
{
    public class AdminController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IAdminRepository _adminRepository;

        public AdminController(IConfiguration configuration, ApplicationDbContext dbContext, IWebHostEnvironment hostEnvironment, IAdminRepository adminRepository)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
            _adminRepository = adminRepository;
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
                var query = $"SELECT * FROM public.\"Administrator\" WHERE public.\"Administrator\".\"EmailAddress\" = @EmailAddress AND public.\"Administrator\".\"Password\" = @Password";

                var parameters = new List<NpgsqlParameter>
                {
                    new NpgsqlParameter("@EmailAddress", EmailAddress),
                    new NpgsqlParameter("@Password", Password),
                };
                Admin administrator = AuthenticateAdministrator(query, parameters);
                if (administrator == null)
                {
                    TempData["error"] = "Invalid Login. User not found";
                }
                else
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, administrator.EmailAddress),
                        new Claim(ClaimTypes.Role, "admin")
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, "AdminAuthentication");

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };

                    await HttpContext.SignInAsync(
                        "AdminAuthentication",
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    TempData["success"] = "Login Successfull";
                    return RedirectToAction("Authenticated");
                }
            }
            return View();
        }

        public Admin? AuthenticateAdministrator(string query, List<NpgsqlParameter> parameters)
        {
            Admin administrator = null;

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                // Prep command object.
                NpgsqlCommand command = new NpgsqlCommand(query, connection);

                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }

                connection.Open();

                // Obtain a data reader via ExecuteReader()
                using (NpgsqlDataReader dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        administrator = new Admin
                        {
                            Name = dataReader["Name"].ToString(),
                            EmailAddress = dataReader["EmailAddress"].ToString()
                        };
                    }
                    dataReader.Close();
                }
            }
            if (administrator == null)
                return null;
            return administrator;
        }

        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync("AdminAuthentication");
            TempData["success"] = "Logout Successfull";
            return RedirectToAction("Login");
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

        [Authorize(AuthenticationSchemes = "AdminAuthentication")]
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
                TempData["error"] = "Error while editing service Point";
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
            await _adminRepository.DeleteServicePoint(id);
            TempData["success"] = "Service Point Deleted Successfully";
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
        public IActionResult DeleteServiceProvider(int id)
        {
            var serviceProvider = _adminRepository.DeleteServiceProvider(id);
            TempData["success"] = "Service Provider Deleted Successfully";
            return RedirectToAction("ServiceProviders");
        }

        [Authorize(AuthenticationSchemes = "AdminAuthentication")]
        public IActionResult AnalyticalReports()
        {
            return View();
        }

        [Authorize(AuthenticationSchemes = "AdminAuthentication")]
        public IActionResult CustomersServed()
        {
            // Load the report file
            string templatePath = $"Reports/CustomersServed.frx";
            var webHostEnvironment = HttpContext.RequestServices.GetService<IWebHostEnvironment>();
            string reportPath = Path.Combine(webHostEnvironment.ContentRootPath, templatePath);
            Report report = new Report();
            report.Load(reportPath);


            // Set up the data connection
            var connectionString = "Host=localhost;Username=postgres;Password=coxmusyoki1233;Database=DBQueue";
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sql = "SELECT COUNT(*) as num_customers FROM public.\"Customers\" WHERE public.\"Customers\".\"Status\" = 'Finished';";
            using var command = new NpgsqlCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                report.SetParameterValue("num_customers", reader.GetInt64(0));
            }

            // Prepare the report data
            MemoryStream stream = new MemoryStream();
            report.Prepare();

            var export = new PDFSimpleExport();
            // export.Compressed = true;

            export.Export(report, stream);
            var pdfBytes = stream.ToArray();
            // Return the report as a file
            return File(pdfBytes, "application/pdf", "CustomersServed.pdf");

        }

        [Authorize(AuthenticationSchemes = "AdminAuthentication")]
        public IActionResult AvgWaitingTime()
        {
            // Load the report file
            string templatePath = $"Reports/AverageWaiting.frx";
            var webHostEnvironment = HttpContext.RequestServices.GetService<IWebHostEnvironment>();
            string reportPath = Path.Combine(webHostEnvironment.ContentRootPath, templatePath);
            Report report = new Report();
            report.Load(reportPath);


            // Set up the data connection
            var connectionString = "Host=localhost;Username=postgres;Password=coxmusyoki1233;Database=DBQueue";
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sql = "SELECT AVG(EXTRACT(EPOCH FROM (\"Customers\".\"CallTime\" - \"Customers\".\"CheckInTime\"))) / 60 as avg_waiting_time, AVG(EXTRACT(EPOCH FROM (\"Customers\".\"EndServiceTime\" - \"Customers\".\"StartServiceTime\"))) / 60 as avg_service_time, \"Customers\".\"ServicePointId\" FROM public.\"Customers\" WHERE \"Customers\".\"Status\" = 'Finished' GROUP BY \"Customers\".\"ServicePointId\";";
            using var command = new NpgsqlCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                // report.SetParameterValue("service_point", reader.GetDouble(0));
                report.SetParameterValue("avg_waiting_time", Math.Round(reader.GetDouble(0)));
                report.SetParameterValue("avg_service_time", Math.Round(reader.GetDouble(1)));
            }

            // Prepare the report data
            MemoryStream stream = new MemoryStream();
            report.Prepare();

            var export = new PDFSimpleExport();
            // export.Compressed = true;

            export.Export(report, stream);
            var pdfBytes = stream.ToArray();
            // Return the report as a file
            return File(pdfBytes, "application/pdf", "WaitingTime.pdf");
        }
    }
}