using Queue_Management_System.Models;
using Queue_Management_System.ServiceInterface;

namespace Queue_Management_System.Services
{
    public class AuthService : IAuthService
    {
        private readonly IServiceProviderRepository _userRepository;

        public AuthService(IServiceProviderRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ServiceProviderModel> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username, password);

            // Implement password verification logic here using user.PasswordHash
            if (user != null)
            {
                return user;
            }

            return null;
        }

    }
}
