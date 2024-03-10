using FastReport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Queue_Management_System.Constants;
using Queue_Management_System.Data;
using Queue_Management_System.Models;
using System;

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
            var zones = _dbContext.Providers.Select(b => b.Name).ToList(); ;
            ViewBag.Points = new SelectList(zones);
            return View();
        }

        [HttpGet]
        public IActionResult Service_Provider()
        {
            
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddServicePoint(Spoints point)
        {
            
                _dbContext.Points.Add(new Spoints
                {
                    Counter = point.Counter,
                    ServiceProvider = point.ServiceProvider,
                    Status = true,
                    

                });
                _dbContext.SaveChanges();
            
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
        [ValidateAntiForgeryToken]
        public IActionResult Service_Provider(Sprovider provider)
        {
           
                _dbContext.Providers.Add(new Sprovider
                {
                    Name = provider.Name,
                    Status= "Open",
                    


                });
                _dbContext.SaveChanges();
            
            return RedirectToAction(nameof(Service_Providers));

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
        
            [HttpPost]
        public IActionResult CloseProvider(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var provider = _dbContext.Providers.FirstOrDefault(n => n.id == id);
            if (provider == null)
            {
                return NotFound();
            }
            provider.Name = provider.Name;
            provider.Status = "Closed";
            _dbContext.Update(provider);
            _dbContext.SaveChangesAsync();
            return View();
        }
        public FileResult Analytics()
            {
               
                var allCustomers = _dbContext.Customers.Count();
                var waitingTime = "12 mins";
                var serviceTime = "14 mins";
                FastReport.Utils.Config.WebMode = true;
                Report rep = new Report();
                string path = Path.Combine(_hostingEnvironment.ContentRootPath, "Analytics.frx");
                rep.Load(path);
            

            
                rep.SetParameterValue("param1", allCustomers);
                rep.SetParameterValue("param2", waitingTime);
                rep.SetParameterValue("param3", serviceTime);
                rep.RegisterData("", "CustomerServed");
                if (rep.Report.Prepare())
                {
                    FastReport.Export.PdfSimple.PDFSimpleExport pdfExport = new FastReport.Export.PdfSimple.PDFSimpleExport();
                    pdfExport.ShowProgress = false;
                    pdfExport.Subject = "Analysis Report";
                    pdfExport.Title = "CUstomer Service Analysis";
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    rep.Report.Export(pdfExport, ms);
                    rep.Dispose();
                    ms.Position = 0;
                    return File(ms, "application/pdf", "Analytics.pdf");
                }
                else
                {
                    return null;
                }



            
        }

    }
}
