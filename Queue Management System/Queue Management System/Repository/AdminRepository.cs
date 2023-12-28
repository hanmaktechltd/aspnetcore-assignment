using Npgsql;
using Queue_Management_System.Models;
using Queue_Management_System.Services;

namespace Queue_Management_System.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private const string _serviceProvidersTable = "appusers";
        private const string _servicePointTable = "servicepoints";
        private IConfiguration _config;
        private NpgsqlConnection _connection;

        public AdminRepository(IConfiguration config)
        {
            _config = config;
        }
        private void OpenConnection()
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");

            _connection = new NpgsqlConnection(connectionString);
            _connection.Open();
        }
        public async Task<IEnumerable<ServiceProviderM>> GetServiceProviders()
        {
            OpenConnection();
            List<ServiceProviderM> serviceProviders = new List<ServiceProviderM>();
            string commandText = $"SELECT id, name FROM {_serviceProvidersTable} WHERE role = 'Service Provider'";
            using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, _connection))
            {
                using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {

                        ServiceProviderM serviceProvider = new ServiceProviderM
                        {
                            Id = (int)reader["id"],
                            Name = (string)reader["name"]
                        };
                        serviceProviders.Add(serviceProvider);
                    }
                    reader.Close();
                }
                _connection.Close();
            }
            if (serviceProviders.Count() == 0)
            {
                return null;
            }
            return serviceProviders;
        }
        public async Task<ServiceProviderM> GetServiceProviderDetails(int id)
        {
            OpenConnection();

                ServiceProviderM serviceProviderDetails = null;

            string commandText = $"SELECT * FROM {_serviceProvidersTable} WHERE id = @id";
            using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, _connection))
            {
               
                cmd.Parameters.AddWithValue("id", id);

               
                using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    
                        if (await reader.ReadAsync())
                        {
                            
                            serviceProviderDetails = new ServiceProviderM
                            {
                                Id = (int)reader["id"],
                                Name = (string)reader["name"],
                                Password = (string)reader["password"],
                                Role = (string)reader["role"]
                            };
                        }
                    reader.Close();
                }
                
                _connection.Close();
                return serviceProviderDetails;
            }
            return null;
        }
        public async Task CreateServiceProvider(ServiceProviderM serviceProvider)
        {
            OpenConnection();
            string commandText = $"INSERT INTO {_serviceProvidersTable} (name, password, role, servicepointid) VALUES (@name, @password, 'Service Provider', @servicepointid)";

            
            using (var cmd = new NpgsqlCommand(commandText, _connection))
            {
                cmd.Parameters.AddWithValue("name", serviceProvider.Name);
                cmd.Parameters.AddWithValue("password", serviceProvider.Password);
                cmd.Parameters.AddWithValue("servicepointid", serviceProvider.ServicepointId);

                await cmd.ExecuteNonQueryAsync();
            }
          
            _connection.Close();
        }
        public async Task UpdateServiceProvider(int id, ServiceProviderM serviceProvider)
        {
            OpenConnection();

            string commandText = $@"UPDATE {_serviceProvidersTable} SET password = @password, servicepointid = @servicepointid WHERE id = @id";

            
            using (var cmd = new NpgsqlCommand(commandText, _connection))
            {
                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("password", serviceProvider.Password);
                cmd.Parameters.AddWithValue("servicepointid", serviceProvider.ServicepointId);

                await cmd.ExecuteNonQueryAsync();
            }
            
            _connection.Close();
        }
        public async Task DeleteServiceProvider(int id)
        {
            OpenConnection();
           
            string commandText = $"DELETE FROM {_serviceProvidersTable} WHERE ID=(@id)";
            using (var cmd = new NpgsqlCommand(commandText, _connection))
            {
               
                cmd.Parameters.AddWithValue("id", id);

                await cmd.ExecuteNonQueryAsync();
            }
            
            _connection.Close();
        }
        public async Task<IEnumerable<ServicePointM>> GetServicePoints()
        {
            OpenConnection();
            List<ServicePointM> servicePoints = new List<ServicePointM>();

            string commandText = $"SELECT * FROM {_servicePointTable}";
            
            using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, _connection))
            {
                
                using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                       
                        ServicePointM servicePoint = new ServicePointM
                        {
                            Id = (int)reader["id"],
                            Name = (string)reader["name"],
                            ServiceProviderId = (int)reader["serviceproviderid"]
                        };
                        servicePoints.Add(servicePoint);
                    }
                    reader.Close();
                }
                _connection.Close();
            }
            if (servicePoints.Count() == 0)
            {
                return null;
            }
            return servicePoints;
        }
        public async Task<ServicePointM> GetServicePointDetails(int id)
        {
            OpenConnection();


            ServicePointM servicePointDetails = null;

            string commandText = $"SELECT * FROM {_servicePointTable} WHERE id = @id";
            using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, _connection))
            {
                
                cmd.Parameters.AddWithValue("id", id);

               
                using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    
                        if (await reader.ReadAsync())
                        {
                         
                            servicePointDetails = new ServicePointM
                            {
                                Id = (int)reader["id"],
                                Name = (string)reader["name"],
                                ServiceProviderId = (int)reader["serviceproviderid"]
                            };
                        }
                    reader.Close();
                }
                
                _connection.Close();
            }
            if (servicePointDetails == null)
            {
                return null;
            }
            return servicePointDetails;
        }
        public async Task CreateServicePoint(ServicePointM servicePoint)
        {
            OpenConnection();
            string commandText = $"INSERT INTO {_servicePointTable} (name, serviceproviderid) VALUES (@name, @serviceproviderid)";

            
            using (var cmd = new NpgsqlCommand(commandText, _connection))
            {
                cmd.Parameters.AddWithValue("name", servicePoint.Name);
                cmd.Parameters.AddWithValue("serviceproviderid", servicePoint.ServiceProviderId);

                await cmd.ExecuteNonQueryAsync();
            }
            
            _connection.Close();
        }
        public async Task UpdateServicePoint(int id, ServicePointM servicePoint)
        {
            OpenConnection();
            
            string commandText = $@"UPDATE {_servicePointTable} SET  serviceproviderid = @serviceproviderid WHERE id = @id";

            using (var cmd = new NpgsqlCommand(commandText, _connection))
            {
                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("serviceproviderid", servicePoint.ServiceProviderId);

                await cmd.ExecuteNonQueryAsync();
            }
            
            _connection.Close();
        }
        public async Task DeleteServicePoint(int id)
        {
            OpenConnection();
            string commandText = $"DELETE FROM {_servicePointTable} WHERE ID=(@p)";
            //await using (var cmd = new NpgsqlCommand(commandText, _connection))
            using (var cmd = new NpgsqlCommand(commandText, _connection))
            {
                cmd.Parameters.AddWithValue("p", id);
                await cmd.ExecuteNonQueryAsync();
            }
            
            _connection.Close();
        }
    }
}
