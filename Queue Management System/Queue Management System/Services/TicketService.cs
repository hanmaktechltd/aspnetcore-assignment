using System.IO;
using FastReport;
using Queue_Management_System.Models;
using Queue_Management_System.Repository;
using Queue_Management_System.ServiceInterface;
using FastReport.Export.PdfSimple;
using System.Drawing.Printing;
using System.Diagnostics;

namespace Queue_Management_System.Services
{
    public class TicketService : ITicketService
    {

        private readonly DbOperationsRepository _dbOperationsRepository;

        public TicketService(DbOperationsRepository dbOperationsRepository)
        {
            _dbOperationsRepository = dbOperationsRepository;
        }

        public async Task<List<ServiceTypeModel>> GetAvailableServicesAsync()
        {
            return await _dbOperationsRepository.GetAvailableServicesAsync();
           
        }
        public async Task<bool> CheckInAsync(string ticketNumber, string serviceName, string customerName, int serviceId)
        {
            try
            {
                bool saved = await _dbOperationsRepository.SaveSelectedService(ticketNumber, serviceName, customerName, serviceId);

                if (saved)
                {
                    // Create a new report instance
                    using (Report report = new Report())
                    {
                         //return true;
                        report.RegisterData("Host=localhost;Port=5433;Database=QueueManagementSystem;Username=postgres;Password=Admin;", "connection");
                        report.Load(@"C:\Users\phill\OneDrive\Documents\Queue System\Ticket.frx");
                       // report.Load(@"C:\Users\phill\OneDrive\Documents\Queue System\Ticket.frx");

                        report.SetParameterValue("TicketNumber", ticketNumber);
                        report.SetParameterValue("ServiceName", serviceName);
                        report.SetParameterValue("CustomerName", customerName);

                        report.Prepare();

                        

                        using (PDFSimpleExport export = new PDFSimpleExport())
                        {
                            string filePath = @"C:\Users\phill\OneDrive\Documents\Queue System\Ticket.pdf";
                            export.Export(report, filePath);
                            PrintPDF(filePath, "printerName");
                        }
                        return true;
                    }

                   
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during check-in and report generation: {ex.Message}");
            }

            return false;

        }
        public ServiceTypeModel GetServiceDetails(int selectedServiceId)
        {
            return new ServiceTypeModel
            {
                Id = selectedServiceId,
                Name = "Service Name",
            };
        }


public void PrintPDF(string filePath, string printerName)
    {
        try
        {
            PrintDocument printDocument = new PrintDocument();

            if (!string.IsNullOrEmpty(printerName))
            {
                printDocument.PrinterSettings.PrinterName = printerName;
            }

            printDocument.PrintPage += (sender, e) =>
            {
                using (Process pdfProcess = new Process())
                {
                    pdfProcess.StartInfo.FileName = filePath;
                    pdfProcess.StartInfo.Verb = "Print";
                    pdfProcess.StartInfo.CreateNoWindow = true;
                    pdfProcess.Start();
                    pdfProcess.WaitForInputIdle();
                    pdfProcess.Kill();
                }
            };

            printDocument.Print();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    


}
}
