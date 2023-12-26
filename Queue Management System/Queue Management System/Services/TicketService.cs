using System.IO;
using FastReport;
using Microsoft.AspNetCore.Connections;
using Queue_Management_System.Models;
using Queue_Management_System.Repository;
using Queue_Management_System.ServiceInterface;

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
            return await _dbOperationsRepository.SaveSelectedService(ticketNumber, serviceName, customerName, serviceId);
        }
        public byte[] GenerateTicket(ServiceTypeModel selectedService)
        {
            // Create a new report
            Report report = new Report();
          //  report.Load("path/to/your/template.frx"); // Replace with your template path

            // Add data to the report
            report.SetParameterValue("ServiceName", selectedService.Name);
            report.SetParameterValue("ServiceId", selectedService.Id);

            // Generate the report
            using (MemoryStream stream = new MemoryStream())
            {
                report.Prepare();
                //report.Export(new FastReport.Export.Pdf.PDFExport(), stream);
                return stream.ToArray();
            }
        }

        // Method to retrieve available services (Replace this with your actual data retrieval logic)
      

        // Mock implementation for fetching service details (Replace with your database access logic)
        public ServiceTypeModel GetServiceDetails(int selectedServiceId)
        {
            return new ServiceTypeModel
            {
                Id = selectedServiceId,
                Name = "Service Name",
                // Other service details...
            };
        }

        public string SaveTicketToFile(byte[] ticketBytes)
        {
           // string filePath = Path.Combine("C:\\Users\\phill\\OneDrive\\Documents\\Queue System", "ticket.pdf");
           // File.WriteAllBytes(filePath, ticketBytes);
            return null;
        }

       
    }
}
