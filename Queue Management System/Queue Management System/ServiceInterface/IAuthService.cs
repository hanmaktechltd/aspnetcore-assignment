using Queue_Management_System.Models;

namespace Queue_Management_System.ServiceInterface
{
    public interface IAuthService
    {
        Task<bool> IsUserAuthorizedAsync(LoginViewModel admin);

    }
}
