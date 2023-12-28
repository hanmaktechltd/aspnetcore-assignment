using Queue_Management_System.Models;

namespace Queue_Management_System.ServiceInterface
{
    public interface ITicketService
    {
        Task<List<ServiceTypeModel>> GetAvailableServicesAsync();
        ServiceTypeModel GetServiceDetails(int selectedServiceId); 
        Task<bool> CheckInAsync(string TicketNumber, string serviceName, string customerName, int serviceId);
    }
}
