using Queue_Management_System.Models;

namespace Queue_Management_System.Services
{
    public interface IAuthenticationService
    {
        ServiceProviderModel AuthenticateUser(string email, string password);

    }

    public class MockAuthenticationService : IAuthenticationService
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
    }
}