using Npgsql;
using Queue_Management_System.Models;
using Queue_Management_System.Repository;
using Queue_Management_System.ServiceInterface;

namespace Queue_Management_System.Services
{
    public class AuthService : IAuthService
    {
        private readonly IServiceProviderRepository _userRepository;

        private readonly DbOperationsRepository _dbOperationsRepository;

        public AuthService(DbOperationsRepository dbOperationsRepository)
        {
            _dbOperationsRepository = dbOperationsRepository;
        }

        public async Task<bool> IsUserAuthorizedAsync(LoginViewModel admin)
        {

            return await _dbOperationsRepository.IsUserAuthorizedAsync(admin);
        }

    }
}
