using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Queue_Management_System.Data;
using Queue_Management_System.Models;
using System.Reflection;
using FastReport;
using Microsoft.Extensions.Hosting.Internal;
using Queue_Management_System.Constants;

namespace Queue_Management_System.Controllers
{
    public class QueueController : Controller
    {
        private readonly QueueDbContext _dbContext;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        public QueueController(QueueDbContext dbContext, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _dbContext= dbContext;  
        }

        [HttpGet]
        public IActionResult Index()
        {
            
            return View();
        }


        [HttpGet]
        public IActionResult Update(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spoint = _dbContext.Customers.FirstOrDefault(n => n.TicketNumber == id);
            if (spoint == null)
            {
                return NotFound();
            }
           
            return View(spoint);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(CustomerService service)
        {
            if (service.TicketNumber <1)
            {
                return NotFound();
            }

            var spoint = _dbContext.Customers.FirstOrDefault(n => n.TicketNumber == service.TicketNumber);
            if (spoint == null)
            {
                return NotFound();
            }
            spoint.TicketNumber = service.TicketNumber;
            spoint.ServiceRequested = service.ServiceRequested;
            spoint.Status = service.Status;
            spoint.serviceDate = service.serviceDate;
            _dbContext.Update(spoint);
            _dbContext.SaveChanges();
            return View(spoint);
        }
        [HttpGet]
        public IActionResult Transfer(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spoint = _dbContext.Customers.FirstOrDefault(n => n.TicketNumber == id);
            if (spoint == null)
            {
                return NotFound();
            }
            var zones = _dbContext.Providers.Select(b => b.Name).ToList(); ;
            ViewBag.Points = new SelectList(zones);

            return View(spoint);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Transfer(CustomerService service)
        {
            if (service.TicketNumber < 1)
            {
                return NotFound();
            }

            var spoint = _dbContext.Customers.FirstOrDefault(n => n.TicketNumber == service.TicketNumber);
            if (spoint == null)
            {
                return NotFound();
            }
            spoint.TicketNumber = service.TicketNumber;
            spoint.ServiceRequested = service.ServiceRequested;
            spoint.Status = service.Status;
            spoint.serviceDate = service.serviceDate;
            _dbContext.Update(spoint);
            _dbContext.SaveChanges();
            return View(spoint);
        }
        [HttpGet]
        public IActionResult Customers()
        {
            var customer = _dbContext.Customers.Where(n=>n.Status=="Pending").ToList().Take(1);
            return View(customer.OrderBy(n=>n.TicketNumber));
        }



        [Authorize(Roles = StrValues.ProviderRole)]
        [HttpGet]
        public IActionResult ServicePoint()
        {
            var customer = _dbContext.Customers.Where(n => n.Status == "Pending").ToList().Take(1);
            return View(customer.OrderBy(n => n.TicketNumber));
        }

      
        

        //Recalling
        [HttpGet]
        public IActionResult Recall()
        {
            var customer = _dbContext.Customers.Where(n => n.Status != "Pending").ToList();
            return View(customer.OrderBy(n => n.TicketNumber));
        }
        [HttpGet]
        public IActionResult Recalling(long? id)
        {
           
            if (id == null)
            {
                return NotFound();
            }

            var customer = _dbContext.Customers.FirstOrDefault(n => n.TicketNumber == id);
            if (customer == null)
            {
                return NotFound();
            }
           

            return View(customer);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Recalling(long TicketNumber, [Bind("TicketNumber,ServiceRequested,Status,serviceDate")] CustomerService customer)
        {
            if (TicketNumber != customer.TicketNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    CustomerService customer1 = new CustomerService();
                    customer1.TicketNumber = customer.TicketNumber;
                    customer1.ServiceRequested = customer.ServiceRequested;
                    customer1.Status = customer.Status;
                    customer1.serviceDate = DateTime.UtcNow;
                    _dbContext.Update(customer1);
                    _dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    
                        return NotFound();
                    
                   
                }
                return RedirectToAction(nameof(Recall));
            }
            return View(customer);
        }

        public FileResult Generate(string? id)
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
            FastReport.Utils.Config.WebMode= true;
            Report rep=new Report();
            string path = Path.Combine(_hostingEnvironment.ContentRootPath, "ServedCustomers.frx");
            rep.Load(path);
            List<CustomerService> cust = new List<CustomerService>();
            cust.Add(new CustomerService() { TicketNumber=newNo,ServiceRequested=id,Status="Pending",serviceDate=DateTime.Now});
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
