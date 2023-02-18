using Npgsql;
using Queue_Management_System.Services;
using Queue_Management_System.Models;

namespace Queue_Management_System.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private const string CONNECTION_STRING = "Host=localhost:5432;" +
                          "Username=postgres;" +
                          "Password=*mikemathu;" +
                          "Database=QMS";

        private const string _serviceProvidersTable = "users";
        private const string _servicePointTable = "servicepoints";

        private NpgsqlConnection connection;
        public AdminRepository()
        {
            connection = new NpgsqlConnection(CONNECTION_STRING);
            connection.Open();
        }
        public async Task<IEnumerable<ServiceProviderVM>> GetServiceProviders()
        {
            List<ServiceProviderVM> serviceProviders = new List<ServiceProviderVM>();

            string commandText = $"SELECT * FROM {_serviceProvidersTable} WHERE role = 'Service Provider'";
            await using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, connection))
            await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                {
                    ServiceProviderVM serviceProvider = ReadServiceProviders(reader);
                    serviceProviders.Add(serviceProvider);
                }
            return serviceProviders;
        }
        private static ServiceProviderVM ReadServiceProviders(NpgsqlDataReader reader)
        {
            int? id = reader["id"] as int?;
            string name = reader["name"] as string;
            string role = reader["role"] as string;
            ServiceProviderVM serviceProviders = new ServiceProviderVM
            {
                Id = (int)id,
                Name = name,
                Role = role
            };
            return serviceProviders;
        }
        public async Task<ServiceProviderVM> GetServiceProviderDetails(int id)
        {
            string commandText = $"SELECT * FROM {_serviceProvidersTable} WHERE ID = @Id";
            await using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, connection))
            {
                cmd.Parameters.AddWithValue("Id", id);

                await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        ServiceProviderVM ServiceProviderDetails = ReadServiceProviderDetails(reader);
                        return ServiceProviderDetails;
                    }
            }
            return null;
        }
        private static ServiceProviderVM ReadServiceProviderDetails(NpgsqlDataReader reader)
        {
            int? id = reader["id"] as int?;
            string name = reader["name"] as string;
            string password = reader["password"] as string;
            string role = reader["role"] as string;
            ServiceProviderVM ServiceProviderDetails = new ServiceProviderVM
            {
                Id = (int)id,
                Name = name,
                Password = password,
                Role = role
            };
            return ServiceProviderDetails;
        }
        public async Task CreateServiceProvider(ServiceProviderVM serviceProvider)
        {
            string commandText = $"INSERT INTO {_serviceProvidersTable} (name, password, role, servicepointid) VALUES (@name, @password, 'Service Provider', @servicepointid)";

            await using (var cmd = new NpgsqlCommand(commandText, connection))
            {
                cmd.Parameters.AddWithValue("name", serviceProvider.Name);
                cmd.Parameters.AddWithValue("password", serviceProvider.Password);
                cmd.Parameters.AddWithValue("servicepointid", serviceProvider.ServicepointId);

                await cmd.ExecuteNonQueryAsync();
            }
        }
        public async Task UpdateServiceProvider(int id, ServiceProviderVM serviceProvider)
        {
            var commandText = $@"UPDATE {_serviceProvidersTable} SET password = @password, servicepointid = @servicepointid WHERE id = @id";

            await using (var cmd = new NpgsqlCommand(commandText, connection))
            {               
                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("password", serviceProvider.Password);
                cmd.Parameters.AddWithValue("servicepointid", serviceProvider.ServicepointId);

                await cmd.ExecuteNonQueryAsync();
            }
        }
        public async Task DeleteServiceProvider(int id)
        {
            string commandText = $"DELETE FROM {_serviceProvidersTable} WHERE ID=(@p)";
            await using (var cmd = new NpgsqlCommand(commandText, connection))
            {
                cmd.Parameters.AddWithValue("p", id);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<IEnumerable<ServicePointVM>> GetServicePoints()
        {
            List<ServicePointVM> servicePoints = new List<ServicePointVM>();

            string commandText = $"SELECT * FROM {_servicePointTable}";
            await using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, connection))
            await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                {
                    ServicePointVM servicePoint = ReadServicePoints(reader);
                    servicePoints.Add(servicePoint);
                }
            return servicePoints;
        }
        private static ServicePointVM ReadServicePoints(NpgsqlDataReader reader)
        {
            int? id = reader["id"] as int?;
            string name = reader["name"] as string;
            int? serviceproviderId = reader["serviceproviderId"] as int?;
            ServicePointVM servicePoint = new ServicePointVM
            {
                Id = (int)id,
                Name = name,
                ServiceProviderId = (int)serviceproviderId
            };
            return servicePoint;
        }
        public async Task<ServicePointVM> GetServicePointDetails(int id)
        {
            string commandText = $"SELECT * FROM {_servicePointTable} WHERE ID = @Id";
            await using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, connection))
            {
                cmd.Parameters.AddWithValue("Id", id);

                await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        ServicePointVM ServiceProviderDetails = ReadServicePointDetails(reader);
                        return ServiceProviderDetails;
                    }
            }
            return null;
        }
        private static ServicePointVM ReadServicePointDetails(NpgsqlDataReader reader)
        {
            int? id = reader["id"] as int?;
            string name = reader["name"] as string;
            int? serviceproviderid = reader["serviceproviderid"] as int?;
            ServicePointVM ServiceProviderDetails = new ServicePointVM
            {
                Id = (int)id,
                Name = name,
                ServiceProviderId = (int)serviceproviderid
            };
            return ServiceProviderDetails;
        }
        public async Task CreateServicePoint(ServicePointVM servicePoint)
        {
            string commandText = $"INSERT INTO {_servicePointTable} (name, serviceproviderid) VALUES (@name, @serviceproviderid)";

            await using (var cmd = new NpgsqlCommand(commandText, connection))
            {
                cmd.Parameters.AddWithValue("name", servicePoint.Name);
                cmd.Parameters.AddWithValue("serviceproviderid", servicePoint.ServiceProviderId);

                await cmd.ExecuteNonQueryAsync();
            }
        }
        public async Task UpdateServicePoint(int id, ServicePointVM servicePoint)
        {
            var commandText = $@"UPDATE {_servicePointTable} SET  serviceproviderid = @serviceproviderid WHERE id = @id";

            await using (var cmd = new NpgsqlCommand(commandText, connection))
            {
                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("serviceproviderid", servicePoint.ServiceProviderId);

                await cmd.ExecuteNonQueryAsync();
            }
        }
        public async Task DeleteServicePoint(int id)
        {
            string commandText = $"DELETE FROM {_servicePointTable} WHERE ID=(@p)";
            await using (var cmd = new NpgsqlCommand(commandText, connection))
            {
                cmd.Parameters.AddWithValue("p", id);
                await cmd.ExecuteNonQueryAsync();
            }
        }       
    }
}
