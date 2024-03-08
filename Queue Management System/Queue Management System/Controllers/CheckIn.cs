using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;
using System.Linq;

namespace Queue_Management_System.Controllers
{
    public class CheckIn : Controller
    {
        private readonly AppDbContext _context;

        public CheckIn(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CheckIn1(Customer model)
        {
            /*if (!ModelState.IsValid)
            {
                return View(model);
            }*/

            model.CheckInTime = DateTime.UtcNow;
            model.Status = "Waiting";
            _context.customers.Add(model);
            _context.SaveChanges();
            //Upate Waiting page
            var waitModel = new WaitingModel();
            UpdateWaitingPage(waitModel);
            //Update Queue
            var queueModel = new QueueItem();
            UpdateQueuePage(queueModel);
            //
            int custid = model.Id;
            var id = _context.customers.Select(x=>x.Id==custid).FirstOrDefault();
            if (id != null)
            {
                PrintTicket(custid);
            }
            
            return RedirectToAction("CheckIn", "Home");
        }
        [HttpPost]
        public IActionResult UpdateWaitingPage(WaitingModel model1)
        {
            var customerInwaiting = _context.customers.ToList();
            try
            {
                foreach (var client in customerInwaiting)
                {
                    if ((client != null) && (client.Status == "Waiting"))
                    {
                        var servicePoint = _context.ServicePoints.Where(sp => sp.Status == "Open").ToList();
                        if (servicePoint != null)
                        {
                            foreach (var queue in servicePoint)
                            {
                                model1.TicketNumber = client.Id;
                                model1.ServicePoint = queue.Id;
                                model1.ServicePointName = queue.Name;
                                _context.waitingModels.Add(model1);
                                _context.SaveChanges();
                                //update Service Point Status
                                queue.Status = "Busy";
                                _context.ServicePoints.Update(queue);
                                _context.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return View();
        }
        
            [HttpPost]
        public IActionResult UpdateQueuePage(QueueItem queue)
        {
            var customerInwaiting = _context.customers.ToList();
            try
            {
                foreach (var client in customerInwaiting)
                {
                    if ((client != null) && (client.Status == "Waiting"))
                    {
                        var servicePoint = _context.ServicePoints.Where(sp => sp.Status == "Busy").ToList();
                        if (servicePoint != null)
                        {
                            foreach (var queuePoint in servicePoint)
                            {
                                queue.TicketNumber = client.Id;
                                queue.ServicePoint = queuePoint.Id;
                                queue.ServicepointName = queuePoint.Name;
                                queue.NoShow = true;
                                queue.Finished = false;
                                _context.QueueItems.Add(queue);
                                _context.SaveChanges();
                                
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return View();
        }
        public IActionResult PrintTicket(int customerId)
        {
            // Fetch customer details from the database based on customerId
            var customer = _context.customers.FirstOrDefault(c => c.Id == customerId);           
            var report = new FastReport.Report();
            report.Load(@"C:\TicketsTemplate.frx");
            // Populate the report with data
            report.SetParameterValue("CustomerName", customer?.Name);
            report.SetParameterValue("ServiceType", customer?.ServiceType);
            report.SetParameterValue("Check In time", customer?.CheckInTime);
            // Render the report
            var pdfStream = new MemoryStream();
            report.Export(new FastReport.Export.Pdf.PDFExport(), pdfStream);
            return File(pdfStream.ToArray(), "application/pdf", "Ticket.pdf");
        }

    }
}
