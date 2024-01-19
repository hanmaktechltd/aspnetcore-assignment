using Queue_Management_System.Models;

namespace Queue_Management_System.Repositories
{
    public interface IServiceProviderRepository
    {
        ServiceProviderModel GetServiceProviderById(string id);

        ServiceProviderModel GetServiceProviderByEmail(string email);

        IEnumerable<ServiceProviderModel> GetServiceProviders();
        
        void AddServiceProvider(ServiceProviderModel serviceProvider);

        void UpdateServiceProvider(ServiceProviderModel serviceProvider);

        void DeleteServiceProvider(ServiceProviderModel serviceProvider);
        
    }

    /*public class ServiceProviderRepository : IServiceProviderRepository
    {

    }
    */
}