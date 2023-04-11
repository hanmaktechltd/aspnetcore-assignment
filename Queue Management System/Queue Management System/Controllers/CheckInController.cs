using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.Extensions.Hosting.Internal;
using Queue_Management_System.Models.Data;

namespace Queue_Management_System.Controllers
{
    public class CheckInController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CheckInController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var servicePoints = _dbContext.ServicePoints.ToList();
            return View(servicePoints);
        }

        public IActionResult Ticket()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CheckIn(int servicePointId)
        {
            // Create a new customer ticket
            var ticket = new CustomerTicket
            {
                ServicePointId = servicePointId,
                ServiceProviderId = servicePointId,
                CheckInTime = DateTime.Now.ToUniversalTime(),
                IsCalled = false,
                NoShow = false,
                Status = "Waiting",
                Completed = false
            };

            // Add the ticket to the database
            _dbContext.Customers.Add(ticket);
            _dbContext.SaveChanges();

            // Load the FastReport.Net template file dynamically
            string templatePath = $"Reports/Ticket.frx";
            var webHostEnvironment = HttpContext.RequestServices.GetService<IWebHostEnvironment>();
            var physicalPath = Path.Combine(webHostEnvironment.ContentRootPath, templatePath);
            Report report = new Report();
            report.Load(physicalPath);

            // Populate the template with customer information
            report.SetParameterValue("TicketNumber", ticket.Id);
            report.SetParameterValue("CheckInTime", ticket.CheckInTime.ToString("g"));

            // Display the generated ticket on the Check-In page
            MemoryStream stream = new MemoryStream();
            report.Prepare();

            var export = new PDFSimpleExport();
            // export.Compressed = true;

            export.Export(report, stream);
            var pdfBytes = stream.ToArray();
            // Response.ContentType = "application/pdf";
            // Response.Body.WriteAsync(pdfBytes, 0, pdfBytes.Length);

            stream.Flush();
            stream.Position = 0;
            TempData["success"] = "Ticket Generated Successfully";
            return File(pdfBytes, "application/pdf", $"Ticket-{ticket.Id}.pdf");
        }

    }
}
