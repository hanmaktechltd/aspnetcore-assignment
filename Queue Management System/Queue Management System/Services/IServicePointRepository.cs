using Queue_Management_System.Models;

namespace Queue_Management_System.Services
{
    public interface IServicePointRepository
    {
        Task<List<CustomerTicket>> GetQueueStatus(int? servicePointId);
        Task<CustomerTicket> GetNextNumber(int? servicePointId);
        // Task<ServicePoint> RecallNumber();
        Task MarkAsNoShow(int Id, int? servicePointId);
        // Task MarkAsFinished(int Id);
        // Task TransferNumber(int currentServicePointId, int servicePointIdTranser);
    }
}
