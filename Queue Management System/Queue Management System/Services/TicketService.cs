using System.IO;
using FastReport;
using Microsoft.AspNetCore.Connections;
using Queue_Management_System.Models;
using Queue_Management_System.Repository;
using Queue_Management_System.ServiceInterface;
using FastReport.Export.PdfSimple;

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
        bool saved = await _dbOperationsRepository.SaveSelectedService(ticketNumber, serviceName, customerName, serviceId);

        if (saved)
        {
            // Create a new report instance
            using (Report report = new Report())
            {
                report.Load(@"C:\Users\phill\OneDrive\Documents\Queue System\Ticket.frx");


                report.SetParameterValue("TicketNumber", ticketNumber);
                report.SetParameterValue("ServiceName", serviceName);
                report.SetParameterValue("CustomerName", customerName);



                report.Prepare();

                using (PDFSimpleExport export = new PDFSimpleExport())
                {
                    export.Export(report, @"C:\Users\phill\OneDrive\Documents\Queue System\Ticket.pdf");
                }
            }

            return true;
        }

        return false;
    }




    public ServiceTypeModel GetServiceDetails(int selectedServiceId)
        {
            return new ServiceTypeModel
            {
                Id = selectedServiceId,
                Name = "Service Name",
                // Other service details...
            };
        }

       
       
    }
}
