using Queue_Management_System.Models;
using Npgsql;

namespace Queue_Management_System.Repositories
{
    public interface IServiceRepository
    {
        Task<ServiceModel> GetServiceById(string id);

        Task<IEnumerable<ServiceModel>> GetServices();

        Task AddService(ServiceModel service);

        Task UpdateService(ServiceModel service);

        Task DeleteService(string id);
    }

    public class ServiceRepository : IServiceRepository
    {

        private readonly IConfiguration _configuration;
        
        private readonly NpgsqlDataSource dataSource;

        public ServiceRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            string connectionString = _configuration.GetConnectionString("qmsdb");
            dataSource = NpgsqlDataSource.Create(connectionString);
        }

        public async Task<ServiceModel> GetServiceById(string id)
        {
            
            await using var connection = await dataSource.OpenConnectionAsync();
            //string querystring = $"SELECT * FROM services WHERE id='{id}'";
            await using var command = new NpgsqlCommand("SELECT * FROM services WHERE id=$1", connection)
            {
                Parameters = 
                {
                    new() {Value = id}
                }
            };
            await using var reader = await command.ExecuteReaderAsync();

            ServiceModel? service = null;
            while (await reader.ReadAsync())
            {
                service = new ServiceModel
                {
                    Id = reader.GetString(0),
                    Description = reader.GetString(1) 
                };
            }

            return service;

        }

        public async Task<IEnumerable<ServiceModel>> GetServices()
        {
            await using var connection = await dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("SELECT * FROM services", connection);
            await using var reader = await command.ExecuteReaderAsync();

            var services = new List<ServiceModel>();
            while (await reader.ReadAsync())
            {
                var service = new ServiceModel
                {
                    Id = reader.GetString(0),
                    Description = reader.GetString(1),
                };

                services.Add(service);
            }

            return services;
        }

        public async Task AddService(ServiceModel service)
        {
            string id = service.Id;
            string description = service.Description;

            await using var connection = await dataSource.OpenConnectionAsync();
            //string querystring = $"INSERT INTO services (id, description) VALUES ('{id}', '{description}')";
            await using var command = new NpgsqlCommand("INSERT INTO services (id, description) VALUES ($1, $2)", connection)
            {
                Parameters = 
                {
                    new() {Value = id},
                    new() {Value = description}
                }
            };
            await command.ExecuteNonQueryAsync();

        }

        public async Task UpdateService(ServiceModel service)
        {
            string id = service.Id;
            string description = service.Description;

            await using var connection = await dataSource.OpenConnectionAsync();
            //string querystring = $"INSERT INTO services (id, description) VALUES ('{id}', '{description}')";
            await using var command = new NpgsqlCommand("UPDATE services SET (description) = ($1) WHERE id = $2", connection)
            {
                Parameters = 
                {
                    new() {Value = description},
                    new() {Value = id}
                }
            };
            await command.ExecuteNonQueryAsync();

        }

        public async Task DeleteService(string id)
        {
            await using var connection = await dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("DELETE FROM services WHERE id = $1", connection)
            {
                Parameters = 
                {
                    new() {Value = id}
                }
            };
            await command.ExecuteNonQueryAsync();
        }
    }

    /*public class MockServiceRepository : IServiceRepository
    {
        ServiceModel SampleService1 = new ServiceModel
        {
            Id = "Service1",
            Description = "Description for Service 1",
        };

        ServiceModel SampleService2 = new ServiceModel
        {
            Id = "Service2",
            Description = "Description for Service 2",
        };

        ServiceModel SampleService3 = new ServiceModel
        {
            Id = "Service3",
            Description = "Description for Service 3",
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

         public void UpdateService(ServiceModel service)
        {
            
        }

         public void DeleteService(string id)
        {
            
        }
    }*/
}

