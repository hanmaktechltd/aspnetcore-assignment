using FastReport;
using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;
using Queue_Management_System.Models.ViewModels;
using Queue_Management_System.Repository;
using System.Security.Cryptography;
using System.Text;

namespace Queue_Management_System.Services
{
    public class QueueService
    {
        private readonly QueueRepository _repository;

        public QueueService(
             QueueRepository repository
            )
        {
            _repository = repository;
        }
        //count customers in queue for servicepoint
        public int GetCustomersInQueueByServicePoint(int serviceId)
        {
            var customersinQueue = _repository.CountCustomersinQueueByServicePoint(serviceId);
            return  customersinQueue;
        }
        public int GetTotalCustomersServed()
        {
            var totalCustomersServed = _repository.CountTotalCustomersServed();
            return totalCustomersServed;
        }

        public MemoryStream GenerateTicket(List<TicketViewModel> ticket)
        {
            // Create a new report
            var report = new Report();

            // Load a report template
            report.Load("report.frx");
            report.RegisterData(ticket, "customerName");     
                

            // Set report data
            report.SetParameterValue("parameter1", "value1");
            report.SetParameterValue("parameter2", "value2");
            if (report.Prepare())
            {
                FastReport.Export.PdfSimple.PDFSimpleExport pdfExport= new FastReport.Export.PdfSimple.PDFSimpleExport();
                pdfExport.ShowProgress = false;
                pdfExport.Subject = "Ticket";
                pdfExport.Title = "Name";
                var pdfStream = new MemoryStream();
                report.Report.Export(pdfExport, pdfStream);
                report.Dispose();
                pdfExport.Dispose();
                pdfStream.Position = 0;
                return pdfStream;
            }
        return null;
        }
        public int GetServedCustomersByServicePoint(FilterModel filterModel) 
        {
            var servedCustomers = _repository.CountServedCustomersByServicePoint(filterModel);
            return servedCustomers;
        }
        public string GetAverageWaitTimeByServicePoint(FilterModel filterModel)
        {
            var averageWaitTime = _repository.CalculateAverageWaitTime(filterModel);
            return averageWaitTime.ToString();
        }
        public string GetAverageServiceTimeByServicePoint(FilterModel filterModel)
        {
            var averageWaitTime = _repository.CalculateAverageServiceTime(filterModel);
            return averageWaitTime.ToString();
        }
        public MemoryStream GenerateAnalyticsReport(List<ReportModel> reportModel)
        {
            var report = new Report();

            report.Load("CustomersReport.frx");
            report.RegisterData(reportModel, "analytics");

            report.SetParameterValue("parameter1", "value1");
            report.SetParameterValue("parameter2", "value2");
            if (report.Prepare())
            {
                FastReport.Export.PdfSimple.PDFSimpleExport pdfExport = new FastReport.Export.PdfSimple.PDFSimpleExport();
                pdfExport.ShowProgress = false;
                pdfExport.Subject = "Analytics Report";
                pdfExport.Title = "Analytics Report";
                var pdfStream = new MemoryStream();
                report.Report.Export(pdfExport, pdfStream);
                report.Dispose();
                pdfExport.Dispose();
                pdfStream.Position = 0;
                return pdfStream;
            }
            return null;
        }
        public async Task<string> HashPassword(string password)
        {
            string saltString = "queueforreal";
            byte[] salt = Encoding.UTF8.GetBytes(saltString);

            using var sha256 = SHA256.Create();

            // Combine password and salt
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] passwordAndSaltBytes = new byte[passwordBytes.Length + salt.Length];
            Array.Copy(passwordBytes, passwordAndSaltBytes, passwordBytes.Length);
            Array.Copy(salt, 0, passwordAndSaltBytes, passwordBytes.Length, salt.Length);

            // Hash the password and salt
            byte[] hash = sha256.ComputeHash(passwordAndSaltBytes);
            return Convert.ToBase64String(hash);
        }
    }
}
