using Queue_Management_System.Models;

namespace Queue_Management_System.ServiceInterface
{
    public interface IServiceProviderRepository
    {
        Task<ServiceProviderModel> GetUserByUsernameAsync(string username, string password);
    }
}
