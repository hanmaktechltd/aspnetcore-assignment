using Queue_Management_System.Models;
using Queue_Management_System.ServiceInterface;
using System.Threading.Tasks;

namespace Queue_Management_System.Repository
{
    public class ServiceProviderRepository : IServiceProviderRepository
    {
        private readonly DbOperationsRepository _dbOperationsRepository;

        public ServiceProviderRepository(DbOperationsRepository dbOperationsRepository)
        {
            _dbOperationsRepository = dbOperationsRepository;
        }

        public async Task<ServiceProviderModel> GetUserByUsernameAsync(string username, string password)
        {
            // Assuming LoginAsync returns a ServiceProviderModel based on the username and password
            return await _dbOperationsRepository.LoginAsync(username, password);
        }

        // Other methods for user management based on the IUserRepository interface
    }
}
