using Queue_Management_System.Models;
using Npgsql;

namespace Queue_Management_System.Repositories
{
    public interface IServiceProviderRepository
    {
        Task<ServiceProviderModel> GetServiceProviderById(string id);

        Task<ServiceProviderModel> GetServiceProviderByEmail(string email);

        Task<IEnumerable<ServiceProviderModel>> GetServiceProviders();
        
        Task AddServiceProvider(ServiceProviderModel serviceProvider);

        Task UpdateServiceProvider(ServiceProviderModel serviceProvider);

        Task DeleteServiceProvider(string id);
        
    }

    public class ServiceProviderRepository : IServiceProviderRepository
    {
        private readonly IConfiguration _configuration;
        
        private readonly NpgsqlDataSource dataSource;

        public ServiceProviderRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            string connectionString = _configuration.GetConnectionString("qmsdb");
            dataSource = NpgsqlDataSource.Create(connectionString);
        }

        public async Task<ServiceProviderModel> GetServiceProviderById(string id)
        {
            await using var connection = await dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("SELECT * FROM service_providers WHERE id=$1", connection)
            {
                Parameters = 
                {
                    new() {Value = id}
                }
            };

            await using var reader = await command.ExecuteReaderAsync();

            ServiceProviderModel? serviceProvider = null;
            while (await reader.ReadAsync())
            {
                serviceProvider = new ServiceProviderModel
                {
                    Id = reader.GetString(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    Password = reader.GetString(3),
                    IsAdmin = reader.GetBoolean(4) 
                };
            }

            return serviceProvider;
        }

        public async Task<ServiceProviderModel> GetServiceProviderByEmail(string email)
        {
            await using var connection = await dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("SELECT * FROM service_providers WHERE email=$1", connection)
            {
                Parameters = 
                {
                    new() {Value = email}
                }
            };

            await using var reader = await command.ExecuteReaderAsync();

            ServiceProviderModel? serviceProvider = null;
            while (await reader.ReadAsync())
            {
                serviceProvider = new ServiceProviderModel
                {
                    Id = reader.GetString(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    Password = reader.GetString(3),
                    IsAdmin = reader.GetBoolean(4) 
                };
            }

            return serviceProvider;
        }
    
        public async Task<IEnumerable<ServiceProviderModel>> GetServiceProviders()
        {
            await using var connection = await dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("SELECT * FROM service_providers", connection);
            await using var reader = await command.ExecuteReaderAsync();

            var serviceProviders = new List<ServiceProviderModel>();
            while (await reader.ReadAsync())
            {
                var serviceProvider = new ServiceProviderModel
                {
                    Id = reader.GetString(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    Password = reader.GetString(3),
                    IsAdmin = reader.GetBoolean(4),
                };

                serviceProviders.Add(serviceProvider);
            }

            return serviceProviders;
        }
        
        public async Task AddServiceProvider(ServiceProviderModel serviceProvider)
        {
            string id = serviceProvider.Id;
            string names = serviceProvider.Name;
            string email = serviceProvider.Email;
            string password = serviceProvider.Password;
            bool IsAdmin = serviceProvider.IsAdmin;

            await using var connection = await dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("INSERT INTO service_providers (id, names, email, password, is_administrator) VALUES ($1, $2, $3, $4, $5)", connection)
            {
                Parameters = 
                {
                    new() {Value = id},
                    new() {Value = names},
                    new() {Value = email},
                    new() {Value = password},
                    new() {Value = IsAdmin}
                }
            };
            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateServiceProvider(ServiceProviderModel serviceProvider)
        {
            string id = serviceProvider.Id;
            string names = serviceProvider.Name;
            string email = serviceProvider.Email;
            string password = serviceProvider.Password;
            bool IsAdmin = serviceProvider.IsAdmin;

            await using var connection = await dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("UPDATE service_providers SET (names, email, password, is_administrator) = ($2, $3, $4, $5) WHERE id = $1", connection)
            {
                Parameters = 
                {
                    new() {Value = id},
                    new() {Value = names},
                    new() {Value = email},
                    new() {Value = password},
                    new() {Value = IsAdmin},
                }
            };
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteServiceProvider(string id)
        {
            await using var connection = await dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("DELETE FROM service_providers WHERE id = $1", connection)
            {
                Parameters = 
                {
                    new() {Value = id}
                }
            };
            await command.ExecuteNonQueryAsync();
        }
    }
    
}