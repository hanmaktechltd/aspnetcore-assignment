using Queue_Management_System.Models;

namespace Queue_Management_System.Services
{
    public interface IAdminRepository
    {
        //ServiceProviders
        Task<IEnumerable<ServiceProviderM>> GetServiceProviders();
        Task<ServiceProviderM> GetServiceProviderDetails(int id);
        Task CreateServiceProvider(ServiceProviderM serviceProvider);
        Task UpdateServiceProvider(int id, ServiceProviderM serviceProvider);
        Task DeleteServiceProvider(int id);
        //ServicePoints
        Task<IEnumerable<ServicePointM>> GetServicePoints();
        Task<ServicePointM> GetServicePointDetails(int id);
        Task CreateServicePoint(ServicePointM servicePoint);
        Task UpdateServicePoint(int id, ServicePointM servicePoint);
        Task DeleteServicePoint(int id);
    }
}
