using Queue_Management_System.Models;

namespace Queue_Management_System.Services
{
    public interface IQueueRepository
    {
        Task<IEnumerable<ServicePointM>> GetServices();
        Task AddCustomerToQueue(ServicePointM customer);
        Task<IEnumerable<QueueM>> GetCalledCustomers();
        Task<IEnumerable<QueueM>> GetWaitingCustomers(int servicePointId);
        Task<QueueM> CurrentServingCustomer(int servicePointId);
        Task<QueueM> UpdateOutGoingAndIncomingCustomerStatus(int outgoingCustomerId, int servicePointId);
       // Task<QueueM> GetCurentlyCalledNumber(string serviceProviderId);
        Task<QueueM> MarkNumberASNoShow(int servicePointId);
        Task<QueueM> MarkNumberASFinished(int servicePointId);
        Task<QueueM> TransferNumber(string serviceProviderId, int servicePointid);
    }
}
