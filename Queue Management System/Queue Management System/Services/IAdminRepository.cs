using Queue_Management_System.Models;
using System.Threading.Tasks;

namespace Queue_Management_System.Services
{
    public interface IAdminRepository
    {
        // Service Points
        Task<List<ServicePoint>> GetAllServicePoints();
        Task<ServicePoint> GetServicePointById(int id);
        Task CreateServicePoint(ServicePoint servicePoint);
        Task UpdateServicePoint(ServicePoint servicePoint);
        Task DeleteServicePoint(int id);

        // Service Providers
        Task<List<Models.ServiceProvider>> GetAllServiceProviders();
        Task<Models.ServiceProvider> GetServiceProviderById(int id);
        Task CreateServiceProvider(Models.ServiceProvider serviceProvider);
        Task UpdateServiceProvider(Models.ServiceProvider serviceProvider);
        Task DeleteServiceProvider(int id);
    }
}