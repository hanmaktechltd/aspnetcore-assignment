using Queue_Management_System.Models;
using Queue_Management_System.Repositories;

namespace Queue_Management_System.Services
{
    public interface IAuthenticationService
    {
        Task<ServiceProviderModel>? AuthenticateUser(string email, string password);

    }

    /*public class MockAuthenticationService : IAuthenticationService
    {
        public ServiceProviderModel AuthenticateUser(string email, string password)
        {
            return new ServiceProviderModel(){
                Id = "MockId_001",
                Name = "MockUserName",
                Email = email,
                Password = password
            };
        }
    }*/

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IServiceProviderRepository _serviceProviderRepository;
        public AuthenticationService(IServiceProviderRepository serviceProviderRepository)
        {
            _serviceProviderRepository = serviceProviderRepository;
        }
        public async Task<ServiceProviderModel>? AuthenticateUser(string email, string password)
        {
            ServiceProviderModel? user = await _serviceProviderRepository.GetServiceProviderByEmail(email);

            if (user == null)
            {
                return null;
            }

            if (!String.Equals(user.Password, password))
            {
                return null;
            }

            return user;
        }
    }
}