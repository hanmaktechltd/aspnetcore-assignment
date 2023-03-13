using FastReport;
using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;
using Queue_Management_System.Models.ViewModels;
using Queue_Management_System.Repository;

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
            var servedCustomers = _repository.CountServedCustomersByServicePoint(filterModel.ServicePointId);
            return servedCustomers;
        }
        public string GetAverageWaitTimeByServicePoint(FilterModel filterModel)
        {
            var averageWaitTime = _repository.CalculateAverageWaitTime(filterModel);
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
    }
}
