using Queue_Management_System.Models;

namespace Queue_Management_System.Repositories
{
    public interface IServiceRepository
    {
        ServiceModel GetServiceById(string id);

        IEnumerable<ServiceModel> GetServices();

        void AddService(ServiceModel service);

        void UpdateService(ServiceModel service);

        void DeleteService(string id);
    }

   /* public class ServiceRepository : IServiceRepository
    {

        private readonly IConfiguration _configuration;

        public ServiceRepository(IConfiguration configuration)
        {
            _configuration = configuration;

        }
        public async Task<ServiceModel> GetServiceById(string id)
        {

        }

        public async Task<IEnumerable<ServiceModel>> GetServices()
        {
            
        }

        public async void AddService(ServiceModel service)
        {

        }
    }*/

    public class MockServiceRepository : IServiceRepository
    {
        ServiceModel SampleService1 = new ServiceModel
        {
            ServiceId = "Service1",
            ServiceDescription = "Description for Service 1",
        };

        ServiceModel SampleService2 = new ServiceModel
        {
            ServiceId = "Service2",
            ServiceDescription = "Description for Service 2",
        };

        ServiceModel SampleService3 = new ServiceModel
        {
            ServiceId = "Service3",
            ServiceDescription = "Description for Service 3",
        };

        public ServiceModel GetServiceById(string id)
        {
            return SampleService2;
        }

        public IEnumerable<ServiceModel> GetServices()
        {
            var services = new List<ServiceModel> ()
            {
                SampleService1, SampleService2, SampleService3
            };

            return services;
        }

        public void AddService(ServiceModel service)
        {
            
        }
    }
}

