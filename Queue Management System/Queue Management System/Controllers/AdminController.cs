using FastReport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Queue_Management_System.Constants;
using Queue_Management_System.Data;
using Queue_Management_System.Models;

namespace Queue_Management_System.Controllers
{
    [Authorize(Roles = StrValues.AdminTole)]
    public class AdminController : Controller
    {
        private readonly QueueDbContext _dbContext;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        public AdminController(QueueDbContext dbContext, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            _dbContext= dbContext;
            _hostingEnvironment= hostingEnvironment;
        }
        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult AddServicePoint()
        {
            var zones = _dbContext.Points.ToList();
            ViewBag.Points = new SelectList(zones);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddServicePoint(Spoints point)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Points.Add(new Spoints
                {
                    Counter = point.Counter,
                    ServiceProvider = point.ServiceProvider,
                    Status = true,
                    

                });
                _dbContext.SaveChanges();
            }
            return RedirectToAction(nameof(Service_Points));
            
        }
        [HttpGet]
        public IActionResult Service_Points()
        {
            var spoints = _dbContext.Points.ToList();
            return View(spoints.OrderBy(n => n.id));
        }
        [HttpGet]
        public IActionResult Service_Providers()
        {
            var providers = _dbContext.Providers;
            return View(providers.OrderBy(n => n.id));
        }
        
        [HttpPost]
        public IActionResult Close(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spoint = _dbContext.Points.FirstOrDefault(n => n.id == id);
            if (spoint == null)
            {
                return NotFound();
            }
            spoint.ServiceProvider = spoint.ServiceProvider;
            spoint.Counter=spoint.Counter;
            spoint.Status = false;
             _dbContext.Update(spoint);
            _dbContext.SaveChangesAsync();
            return View();
        }

        public FileResult Analytics(string? id)
            {
                if (id == null)
                {
                    return null;
                }
                var recentNo = _dbContext.Customers.ToList().OrderByDescending(s => s.TicketNumber)
                        .Select(s => s.TicketNumber).FirstOrDefault();
                var newNo = recentNo + 1;
                _dbContext.Customers.Add(new CustomerService
                {
                    TicketNumber = newNo,
                    ServiceRequested = id,
                    Status = "Pending",
                    serviceDate = DateTime.UtcNow,

                });
                _dbContext.SaveChanges();
                FastReport.Utils.Config.WebMode = true;
                Report rep = new Report();
                string path = Path.Combine(_hostingEnvironment.ContentRootPath, "ServedCustomers.frx");
                rep.Load(path);
                List<CustomerService> cust = new List<CustomerService>();
                cust.Add(new CustomerService() { TicketNumber = newNo, ServiceRequested = id, Status = "Pending", serviceDate = DateTime.Now });
                rep.SetParameterValue("param1", "This Ticket is valid within 24 hrs ");
                rep.SetParameterValue("param2", "If you misplaced the ticket,generate another one");
                rep.RegisterData(cust, "CustomerServed");
                if (rep.Report.Prepare())
                {
                    FastReport.Export.PdfSimple.PDFSimpleExport pdfExport = new FastReport.Export.PdfSimple.PDFSimpleExport();
                    pdfExport.ShowProgress = false;
                    pdfExport.Subject = "Subject Report";
                    pdfExport.Title = "Report Title";
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    rep.Report.Export(pdfExport, ms);
                    rep.Dispose();
                    ms.Position = 0;
                    return File(ms, "application/pdf", "ticket.pdf");
                }
                else
                {
                    return null;
                }



            
        }

    }
}
