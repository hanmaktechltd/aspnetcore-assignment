using FastReport;
using FastReport.Export.PdfSimple;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Queue_Management_System.Models;
using Queue_Management_System.Models.Data;
using System.Data;
using System.Diagnostics;

namespace Queue_Management_System.Controllers
{
    public class AdminController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AdminController(IConfiguration configuration, ApplicationDbContext dbContext, IWebHostEnvironment hostEnvironment)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Authenticated()
        {
            return View();
        }

        public IActionResult Login(string EmailAddress, string Password)
        {
            ViewBag.LoginStatus = "";

            if (ModelState.IsValid)
            {
                ViewBag.LoginStatus = "";

                var UserCheck = _dbContext.Administrator.FirstOrDefault
                    (a => a.EmailAddress == EmailAddress && a.Password == Password);

                if (UserCheck == null)
                {
                    ViewBag.LoginStatus = "Invalid Login. User not found";
                }
                else
                {
                    return RedirectToAction("Authenticated");
                }
            }
            return View();
        }
        public IActionResult ServicePoints()
        {
            var servicePoints = _dbContext.ServicePoints.ToList();
            return View(servicePoints);
        }

        public IActionResult ServiceProviders()
        {
            var serviceProviders = _dbContext.ServiceProviders.ToList();
            return View(serviceProviders);
        }

        public IActionResult AddServiceProvider()
        {
            return View();
        }


        public IActionResult AddServicePoint()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddServicePoint(ServicePoint servicePoint)
        {
            _dbContext.ServicePoints.Add(servicePoint);
            _dbContext.SaveChanges();
            return RedirectToAction("ServicePoints");
        }

        public IActionResult EditServicePoint(int id)
        {
            var servicePoint = _dbContext.ServicePoints.Find(id);
            if (servicePoint == null)
            {
                return NotFound();
            }
            return View(servicePoint);
        }

        [HttpPost]
        public IActionResult EditServicePoint(ServicePoint servicePoint)
        {
            _dbContext.ServicePoints.Update(servicePoint);
            _dbContext.SaveChanges();
            return RedirectToAction("ServicePoints");
        }

        public IActionResult DeleteServicePoint(int id)
        {
            var servicePoint = _dbContext.ServicePoints.Find(id);
            if (servicePoint == null)
            {
                return NotFound();
            }
            _dbContext.ServicePoints.Remove(servicePoint);
            _dbContext.SaveChanges();
            return RedirectToAction("ServicePoints");
        }

        [HttpPost]
        public IActionResult AddServiceProvider(Models.ServiceProvider serviceProvider)
        {
            _dbContext.ServiceProviders.Add(serviceProvider);
            _dbContext.SaveChanges();
            return RedirectToAction("ServiceProviders");
        }

        public IActionResult EditServiceProvider(int id)
        {
            var servicePoint = _dbContext.ServicePoints.Find(id);
            if (servicePoint == null)
            {
                return NotFound();
            }
            return View(servicePoint);
        }

        [HttpPost]
        public IActionResult EditServiceProvider(Models.ServiceProvider serviceProvider)
        {
            if (ModelState.IsValid)
            {
                _dbContext.ServiceProviders.Update(serviceProvider);
                _dbContext.SaveChanges();
            }
            return RedirectToAction("ServicePoints");
        }

        public IActionResult DeleteServiceProvider(int id)
        {
            var servicePoint = _dbContext.ServicePoints.Find(id);
            if (servicePoint == null)
            {
                return NotFound();
            }
            _dbContext.ServicePoints.Remove(servicePoint);
            _dbContext.SaveChanges();
            return RedirectToAction("ServicePoints");
        }

        public IActionResult AnalyticalReports()
        {
            return View();
        }

        // public async Task<IActionResult> CustomersServedAsync()
        // {
        //     // Load the report file
        //     var report = new Report();
        //     report.Load("Reports/CustomersServed.frx");

        //     // Create a dataset with the report data
        //     var dataSet = new DataSet();
        //     var dataTable = new DataTable("Customers");
        //     dataSet.Tables.Add(dataTable);
        //     // ... fill the dataset with the required data

        //     // Register the dataset with the report
        //     report.RegisterData(dataSet, "MyDataSet");

        //     // Render the report to a memory stream
        //     var stream = new MemoryStream();
        //     report.Prepare();
        //     report.Export(new PDFSimpleExport(), stream);

        //     // Set the response headers
        //     Response.Clear();
        //     Response.ContentType = "application/pdf";
        //     Response.Headers.Add("content-disposition", "inline; filename=MyReport.pdf");

        //     // Write the report to the response stream
        //     stream.Seek(0, SeekOrigin.Begin);
        //     await stream.CopyToAsync(Response.Body);

        //     return new EmptyResult();
        // // Load the FastReport.Net template file dynamically
        // string templatePath = $"Reports/CustomersServed.frx";
        // var webHostEnvironment = HttpContext.RequestServices.GetService<IWebHostEnvironment>();
        // var physicalPath = Path.Combine(webHostEnvironment.ContentRootPath, templatePath);
        // Report report = new Report();
        // report.Load(physicalPath);

        // // Display the generated ticket on the Check-In page
        // MemoryStream stream = new MemoryStream();
        // report.Prepare();

        // var export = new PDFSimpleExport();
        // // export.Compressed = true;

        // export.Export(report, stream);

        // var pdfBytes = stream.ToArray();
        // // Response.ContentType = "application/pdf";
        // // Response.Body.WriteAsync(pdfBytes, 0, pdfBytes.Length);

        // stream.Flush();
        // stream.Position = 0;
        // return File(pdfBytes, "application/pdf", $"CustomersServed.pdf");

        public IActionResult CustomersServed()
        {
            var webReport = new WebReport();
            var connectionString = _configuration.GetConnectionString("NorthWindConnection");
            var connection = new NpgsqlConnection(connectionString);
            webReport.Report.RegisterData(_dbContext.Customers.ToList(), "Customers", 1);
            webReport.Report.Load(Path.Combine(_hostEnvironment.ContentRootPath, "reports", "CustomersServed.frx"));
            return View(webReport);
        }
    }
}