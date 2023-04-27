using System.Security.Claims;
using BC = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Authentication;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AdminRepository(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<CustomerTicket>> MainQueue()
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            var customers = new List<CustomerTicket>();
            string query = $"SELECT * FROM public.\"Customers\" WHERE \"Status\" = 'Waiting' OR \"Status\" = 'In Progress'";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var customer = new CustomerTicket
                        {
                            Id = (int)reader["Id"],
                            Status = (string)reader["Status"],
                            ServicePointId = (int)reader["ServicePointId"]
                        };

                        customers.Add(customer);
                    }
                }
            }
            if (customers.Count() is 0)
                return null;
            return customers;
        }

        public async Task<List<ServicePoint>> GetAllServicePoints()
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            var servicePoints = new List<ServicePoint>();
            string commandText = $"SELECT * FROM public.\"ServicePoints\"";
            using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection))
            {
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var servicePoint = new ServicePoint
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            Description = (string)reader["Description"],
                            ServiceProviderId = (int)reader["ServiceProviderId"]
                        };

                        servicePoints.Add(servicePoint);
                        // if(servicePoint is ServicePoint)
                    }
                }
            }
            if (servicePoints.Count == 0)
                return null;
            return servicePoints;
        }

        public async Task<ServicePoint?> GetServicePointById(int id)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            ServicePoint servicePoint = null!;
            string query = "SELECT * FROM public.\"ServicePoints\" WHERE public.\"ServicePoints\".\"Id\" =@id";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
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
                            Description = (string)reader["Description"],
                            ServiceProviderId = (int)reader["ServiceProviderId"]
                        };
                    }
                }
            }
            return servicePoint;
        }

        public async Task CreateServicePoint(ServicePoint servicePoint)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            string query = "INSERT INTO public.\"ServicePoints\" (\"Name\", \"Description\", \"ServiceProviderId\")" +
                            "VALUES(@Name, @Description, @serviceProviderId)";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", servicePoint.Name);
                command.Parameters.AddWithValue("@Description", servicePoint.Description);
                command.Parameters.AddWithValue("@serviceProviderId", servicePoint.ServiceProviderId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateServicePoint(ServicePoint servicePoint)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            string query = "UPDATE public.\"ServicePoints\" SET \"Name\" = @name, \"Description\" = @description, \"ServiceProviderId\" = @serviceProviderId WHERE public.\"ServicePoints\".\"Id\" = @id";

            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@name", servicePoint.Name);
                command.Parameters.AddWithValue("@description", servicePoint.Description);
                command.Parameters.AddWithValue("@serviceProviderId", servicePoint.ServiceProviderId);
                command.Parameters.AddWithValue("@Id", servicePoint.Id);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteServicePoint(ServicePoint servicePoint)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            string query = "DELETE FROM public.\"ServicePoints\" WHERE public.\"ServicePoints\".\"Id\" = @id";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", servicePoint.Id);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<List<Models.ServiceProvider>> GetAllServiceProviders()
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            var serviceProviders = new List<Models.ServiceProvider>();
            string query = $"SELECT * FROM public.\"ServiceProviders\"";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
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
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            Models.ServiceProvider serviceProvider = null;
            string query = "SELECT * FROM public.\"ServiceProviders\" WHERE public.\"ServiceProviders\".\"Id\" =@id";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
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
            return serviceProvider;
        }

        public async Task CreateServiceProvider(Models.ServiceProvider serviceProvider)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            string passwordHash = BC.HashPassword(serviceProvider.Password);
            string query = "INSERT INTO public.\"ServiceProviders\" (\"Name\", \"Password\", \"ServicePointId\")" +
                            "VALUES(@Name, @Password, @ServicePointId)";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", serviceProvider.Name);
                command.Parameters.AddWithValue("@Password", passwordHash);
                command.Parameters.AddWithValue("@ServicePointId", serviceProvider.ServicePointId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateServiceProvider(Models.ServiceProvider serviceProvider)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            string passwordHash = BC.HashPassword(serviceProvider.Password);
            string query = "UPDATE public.\"ServiceProviders\" SET \"Name\" = @name, \"Password\" = @Password, \"ServicePointId\" = @ServicePointId WHERE public.\"ServiceProviders\".\"Id\" = @id";

            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", serviceProvider.Name);
                command.Parameters.AddWithValue("@Password", passwordHash);
                command.Parameters.AddWithValue("@ServicePointId", serviceProvider.ServicePointId);
                command.Parameters.AddWithValue("@Id", serviceProvider.Id);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteServiceProvider(Models.ServiceProvider serviceProvider)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            string query = "DELETE FROM public.\"ServiceProviders\" WHERE public.\"ServiceProviders\".\"Id\" = @id";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", serviceProvider.Id);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<Admin> Login(string EmailAddress, string Password)
        {
            var query = $"SELECT * FROM public.\"Administrator\" WHERE public.\"Administrator\".\"EmailAddress\" = @EmailAddress AND public.\"Administrator\".\"Password\" = @Password";

            var parameters = new List<NpgsqlParameter>
                {
                    new NpgsqlParameter("@EmailAddress", EmailAddress),
                    new NpgsqlParameter("@Password", Password),
                };
            Admin administrator = await AuthenticateAdministrator(query, parameters);
            if (administrator == null)
            {
                return null;
            }
            else
            {
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, administrator.EmailAddress),
                        new Claim(ClaimTypes.Role, "admin")
                    };

                var claimsIdentity = new ClaimsIdentity(
                    claims, "AdminAuthentication");

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await _httpContextAccessor.HttpContext.SignInAsync(
                    "AdminAuthentication",
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return administrator;
            }
        }

        public async Task<Admin> AuthenticateAdministrator(string query, List<NpgsqlParameter> parameters)
        {
            Admin administrator = null;

            string connectionString = _config.GetConnectionString("DefaultConnection");

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                // Prep command object.
                NpgsqlCommand command = new NpgsqlCommand(query, connection);

                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }

                connection.Open();

                // Obtain a data reader via ExecuteReader()
                using (NpgsqlDataReader dataReader = await command.ExecuteReaderAsync())
                {
                    if (await dataReader.ReadAsync())
                    {
                        administrator = new Admin
                        {
                            Name = (string)dataReader["Name"],
                            EmailAddress = (string)dataReader["EmailAddress"]
                        };
                    }
                    dataReader.Close();
                }
            }
            return administrator;
        }
    }
}
