using Queue_Management_System.Models;

namespace Queue_Management_System.Services
{
    public interface IQueueRepository
    {
        Task<IEnumerable<ServicePointVM>> GetServices();
        Task AddCustomerToQueue(ServicePointVM customer);       
        Task<IEnumerable<QueueVM>> GetCalledCustomers();
        Task<IEnumerable<QueueVM>> GetWaitingCustomers(string userServingPointId);        
        Task<QueueVM> MyCurrentServingCustomer(string userServingPointId);       
        Task<QueueVM> UpdateOutGoingAndIncomingCustomerStatus(int outgoingCustomerId, string serviceProviderId);
        Task<QueueVM> GetCurentlyCalledNumber(string serviceProviderId);
        Task<QueueVM> MarkNumberASNoShow(string serviceProviderId);
        Task<QueueVM> MarkNumberASFinished(string serviceProviderId);
        Task<QueueVM> TransferNumber(string serviceProviderId, int servicePointid);
    }
}
