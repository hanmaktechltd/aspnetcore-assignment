using Queue_Management_System.Models;

namespace Queue_Management_System.ServiceInterface
{
    public interface IAuthService
    {
        Task<ServiceProviderModel> AuthenticateAsync(string username, string password);
        //Task LogoutAsync();

    }
}
