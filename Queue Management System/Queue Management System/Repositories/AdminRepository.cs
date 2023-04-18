using Npgsql;
using Queue_Management_System.Models;
using Queue_Management_System.Services;

namespace Queue_Management_System.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly string _connectionString;
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

        public async Task<List<ServicePoint>> GetAllServicePoints()
        {
            OpenConnection();
            var servicePoints = new List<ServicePoint>();
            string commandText = $"SELECT * FROM public.\"ServicePoints\"";
            using (NpgsqlCommand command = new NpgsqlCommand(commandText, _connection))
            {
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var servicePoint = new ServicePoint
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            Description = (string)reader["Description"]
                        };

                        servicePoints.Add(servicePoint);
                    }
                    reader.Close();
                }
            }
            if (servicePoints.Count() == 0)
                return null;
            return servicePoints;
        }

        public async Task<ServicePoint> GetServicePointById(int id)
        {
            OpenConnection();
            ServicePoint servicePoint = null;
            string query = "SELECT * FROM public.\"ServicePoints\" WHERE public.\"ServicePoints\".\"Id\" =@id";
            using (NpgsqlCommand command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        servicePoint = new ServicePoint
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            Description = (string)reader["Description"]
                        };
                    }
                    reader.Close();
                }
            }
            if (servicePoint == null)
                return null;
            return servicePoint;
        }

        public async Task CreateServicePoint(ServicePoint servicePoint)
        {
            OpenConnection();
            string query = "INSERT INTO public.\"ServicePoints\" (\"Name\", \"Description\")" +
                            "VALUES(@Name, @Description)";

            using (NpgsqlCommand command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@Name", servicePoint.Name);
                command.Parameters.AddWithValue("@Description", servicePoint.Description);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateServicePoint(ServicePoint servicePoint)
        {
            OpenConnection();
            string query = "UPDATE public.\"ServicePoints\" SET \"Name\" = @name, \"Description\" = @description WHERE public.\"ServicePoints\".\"Id\" = @id";

            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@name", servicePoint.Name);
                command.Parameters.AddWithValue("@description", servicePoint.Description);
                command.Parameters.AddWithValue("@Id", servicePoint.Id);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteServicePoint(int id)
        {
            OpenConnection();
            string query = "DELETE FROM public.\"ServicePoints\" WHERE public.\"ServicePoints\".\"Id\" = @id";
            using (NpgsqlCommand command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@id", id);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<List<Models.ServiceProvider>> GetAllServiceProviders()
        {
            OpenConnection();
            var serviceProviders = new List<Models.ServiceProvider>();
            string query = $"SELECT * FROM public.\"ServiceProviders\"";
            using (NpgsqlCommand command = new NpgsqlCommand(query, _connection))
            {
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var serviceProvider = new Models.ServiceProvider
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            ServicePointId = (int)reader["ServicePointId"]
                        };

                        serviceProviders.Add(serviceProvider);
                    }
                    reader.Close();
                }
            }
            if (serviceProviders.Count() == 0)
                return null;
            return serviceProviders;
        }

        public async Task<Models.ServiceProvider> GetServiceProviderById(int id)
        {
            OpenConnection();
            Models.ServiceProvider serviceProvider = null;
            string query = "SELECT * FROM public.\"ServiceProviders\" WHERE public.\"ServiceProviders\".\"Id\" =@id";
            using (NpgsqlCommand command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        serviceProvider = new Models.ServiceProvider
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            Password = (string)reader["Password"],
                            ServicePointId = (int)reader["ServicePointId"]
                        };
                    }
                    reader.Close();
                }
            }
            if (serviceProvider == null)
                return null;
            return serviceProvider;
        }

        public async Task CreateServiceProvider(Models.ServiceProvider serviceProvider)
        {
            OpenConnection();
            string query = "INSERT INTO public.\"ServiceProviders\" (\"Name\", \"Password\", \"ServicePointId\")" +
                            "VALUES(@Name, @Password, @ServicePointId)";

            using (NpgsqlCommand command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@Name", serviceProvider.Name);
                command.Parameters.AddWithValue("@Password", serviceProvider.Password);
                command.Parameters.AddWithValue("@ServicePointId", serviceProvider.ServicePointId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateServiceProvider(Models.ServiceProvider serviceProvider)
        {
            OpenConnection();
            string query = "UPDATE public.\"ServiceProviders\" SET \"Name\" = @name, \"Password\" = @Password, \"ServicePointId\" = @ServicePointId WHERE public.\"ServiceProviders\".\"Id\" = @id";

            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@Name", serviceProvider.Name);
                command.Parameters.AddWithValue("@Password", serviceProvider.Password);
                command.Parameters.AddWithValue("@ServicePointId", serviceProvider.ServicePointId);
                command.Parameters.AddWithValue("@Id", serviceProvider.Id);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteServiceProvider(int id)
        {
            OpenConnection();
            string query = "DELETE FROM public.\"ServiceProviders\" WHERE public.\"ServiceProviders\".\"Id\" = @id";
            using (NpgsqlCommand command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@id", id);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
