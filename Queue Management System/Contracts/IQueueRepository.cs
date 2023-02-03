using Queue_Management_System.Models;

namespace Queue_Management_System.Contracts
{
    public interface IQueueRepository
    {
        Task<IEnumerable<ServicePointVM>> GetServices();
        Task AddCustomerToQueue(ServicePointVM customer);
        Task<IEnumerable<QueueVM>> GetWaitingCustomers(string userServingPointId);

        //**//
        Task<QueueVM> MyCurrentServingCustomer(string userServingPointId);





        //**//
        Task<QueueVM> UpdateOutGoingAndIncomingCustomerStatus(int outgoingCustomerId, string serviceProviderId); //RENAME: userServingPointId, outgoungcustomerid
    }
}
