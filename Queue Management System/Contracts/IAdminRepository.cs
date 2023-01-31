using Queue_Management_System.Models;

namespace Queue_Management_System.Contracts
{
    public interface IAdminRepository
    {
        Task<IEnumerable<ServiceProviderVM>> GetServiceProviders();
        Task<IEnumerable<ServicePointVM>> GetServicePoints();
        Task<AdminVM> Get(int id);

        Task Add(AdminVM game);

        Task Update(int id, AdminVM game);

        Task Delete(int id);
    }
}
