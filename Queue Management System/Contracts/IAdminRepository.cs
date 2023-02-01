using Queue_Management_System.Models;
using System.Threading.Tasks;

namespace Queue_Management_System.Contracts
{
    public interface IAdminRepository
    {
        Task<IEnumerable<ServiceProviderVM>> GetServiceProviders();
        Task<ServiceProviderVM> GetServiceProviderDetails(int id);
        Task CreateServiceProvider(ServiceProviderVM serviceProvider);
        Task UpdateServiceProvider(int id, ServiceProviderVM serviceProvider);

        //TODO
        //Delete EditServiceProvider

        Task<IEnumerable<ServicePointVM>> GetServicePoints();
        Task<ServicePointVM> GetServicePointDetails(int id);
        Task CreateServicePoint(ServicePointVM servicePoint);
        Task UpdateServicePoint(int id, ServicePointVM servicePoint);









        Task Delete(int id);
    }
}
