using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using FastReport;
using Queue_Management_System.Models;

namespace Queue_Management_System.Controllers
{
    public class ReportController : Controller
    {

        private readonly IWebHostEnvironment _hostingEnvironment;

        public ReportController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public FileResult GenerateCustomerQueueTicket()
        {
            FastReport.Utils.Config.WebMode = true;
            Report rep = new Report();

            //Path to the Customer Ticket
            // Get the absolute path to the wwwroot folder
            string webRootPath = _hostingEnvironment.WebRootPath;
            // Construct the path to the ticket inside the Reports folder
            string customerTicketFilePath = Path.Combine(webRootPath, "Reports", "CustomerQueueTicket.frx");
            // String customerTicketFilePath = Server.MapPath("~/wwwroot/Reports/CustomerQueueTicket.frx");
            rep.Load(customerTicketFilePath);

            if (rep.Report.Prepare())
            {
                FastReport.Export.PdfSimple.PDFSimpleExport pdfExport = new FastReport.Export.PdfSimple.PDFSimpleExport();
                pdfExport.ShowProgress = false;
                pdfExport.Subject = "Subject Report";
                pdfExport.Title = "Report Title";
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                rep.Report.Export(pdfExport, ms);
                rep.Dispose();
                pdfExport.Dispose();
                ms.Position = 0;

                return File(ms, "application/pdf", "customerticket.pdf");
            }
            else
            {
                return null;
            }
        }

        public FileResult GenerateAdminReport()
        {
            FastReport.Utils.Config.WebMode = true;
            Report rep = new Report();

            //Path to the AdminReport
            // Get the absolute path to the wwwroot folder
            string webRootPath = _hostingEnvironment.WebRootPath;
            // Construct the path to the report inside the Reports folder
            string AdminReportFilePath = Path.Combine(webRootPath, "Reports", "AdminReport.frx");
            // String AdminReportFilePath = Server.MapPath("~/wwwroot/Reports/AdminReport.frx");
            rep.Load(AdminReportFilePath);

            if (rep.Report.Prepare())
            {
                FastReport.Export.PdfSimple.PDFSimpleExport pdfExport = new FastReport.Export.PdfSimple.PDFSimpleExport();
                pdfExport.ShowProgress = false;
                pdfExport.Subject = "Subject Report";
                pdfExport.Title = "Report Title";
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                rep.Report.Export(pdfExport, ms);
                rep.Dispose();
                pdfExport.Dispose();
                ms.Position = 0;

                return File(ms, "application/pdf", "adminreport.pdf");
            }
            else
            {
                return null;
            }
        }

    }
}
