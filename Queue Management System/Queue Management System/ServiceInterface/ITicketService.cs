using Queue_Management_System.Models;

namespace Queue_Management_System.ServiceInterface
{
    public interface ITicketService
    {
        byte[] GenerateTicket(ServiceTypeModel selectedService); // Define the method to generate a ticket
        Task<List<ServiceTypeModel>> GetAvailableServicesAsync();
        ServiceTypeModel GetServiceDetails(int selectedServiceId); // Define method to get service details
        string SaveTicketToFile(byte[] ticketBytes); // Define method to save ticket to a file
        Task<bool> CheckInAsync(string TicketNumber, string serviceName, string customerName);
    }
}
