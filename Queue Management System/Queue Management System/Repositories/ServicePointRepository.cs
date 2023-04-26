using System.Security.Claims;
using System.Speech.Synthesis;
using Microsoft.AspNetCore.Authentication;
using BC = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Http;
using Npgsql;
using Queue_Management_System.Models;
using Queue_Management_System.Services;

namespace Queue_Management_System.Repositories
{
    public class ServicePointRepository : IServicePointRepository
    {
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IConfiguration _config;
        private NpgsqlConnection _connection;
        public ServicePointRepository(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<CustomerTicket>> GetQueueStatus(int? servicePointId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            var customers = new List<CustomerTicket>();
            string query = $"SELECT * FROM public.\"Customers\" WHERE public.\"Customers\".\"ServicePointId\" = @ServicePointId AND NOT public.\"Customers\".\"Completed\" AND public.\"Customers\".\"Status\" != 'No Show'";
            // string query = $"SELECT * FROM public.\"Customers\" WHERE public.\"Customers\".\"ServicePointId\" = @ServicePointId";
            using (NpgsqlCommand command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@servicePointId", servicePointId);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var customer = new CustomerTicket
                        {
                            Id = (int)reader["Id"],
                            CheckInTime = (DateTime)reader["CheckInTime"],
                            Status = (string)reader["Status"]
                        };
                        customers.Add(customer);
                    }
                }
            }
            if (customers.Count() is 0)
                return new List<CustomerTicket>();
            return customers;
        }

        public async Task<CustomerTicket> GetNextNumber(int? servicePointId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            // Query the database to retrieve the next customer in the queue with the selected service point ID and status "waiting"
            CustomerTicket nextCustomer = null;
            string query = "SELECT * FROM public.\"Customers\" WHERE public.\"Customers\".\"ServicePointId\" = @ServicePointId AND public.\"Customers\".\"Status\" = 'Waiting'";
            using (NpgsqlCommand command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@servicePointId", servicePointId);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        nextCustomer = new CustomerTicket
                        {
                            Id = (int)reader["Id"],
                            CheckInTime = (DateTime)reader["CheckInTime"],
                            Status = (string)reader["Status"]
                        };
                    }
                }
            }

            // If there is no next customer, display a message to the service provider
            if (nextCustomer is null)
            {
                return null;
            }

            // Update the customer's status to "in progress"
            string updateQuery = "UPDATE public.\"Customers\" SET \"Status\" = 'In Progress', \"StartServiceTime\" = @startServiceTime, \"CallTime\" = @callTime, \"IsCalled\" = @isCalled WHERE public.\"Customers\".\"Id\" = @customerId";
            using (NpgsqlCommand command = new NpgsqlCommand(updateQuery, _connection))
            {
                command.Parameters.AddWithValue("@startServiceTime", DateTime.UtcNow);
                command.Parameters.AddWithValue("@callTime", DateTime.UtcNow);
                command.Parameters.AddWithValue("@isCalled", true);
                command.Parameters.AddWithValue("@customerId", nextCustomer.Id);
                await command.ExecuteNonQueryAsync();
            }
            // Create a new SpeechSynthesizer instance
            using SpeechSynthesizer synth = new SpeechSynthesizer();

            // Set the desired voice by ID
            synth.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult, 0, new System.Globalization.CultureInfo("en-US"));

            // Speak the ticket number and service point ID of the next customer
            string textToSpeak = "Ticket number " + nextCustomer.Id + ", please proceed to room " + servicePointId;
            synth.Speak(textToSpeak);
            // await Task.CompletedTask;
            return nextCustomer;
        }

        public async Task MarkAsNoShow(int Id, int? servicePointId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            CustomerTicket customer = null;
            string query = "SELECT * FROM public.\"Customers\" WHERE public.\"Customers\".\"Id\" = @customerId";
            // string query = "SELECT * FROM \"Customers\" WHERE \"Customers\".\"Id\" = @customerId";
            using (NpgsqlCommand command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@customerId", Id);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        customer = new CustomerTicket
                        {
                            Id = (int)reader["Id"],
                            CheckInTime = (DateTime)reader["CheckInTime"],
                            Status = (string)reader["Status"],
                            NoShow = (bool)reader["NoShow"]
                        };
                    }
                }
            }
            // Update the customer's status to "no show" and remove them from the queue
            string updateQuery = "UPDATE public.\"Customers\" SET \"Status\" = 'No Show', \"NoShow\" = @noShow WHERE public.\"Customers\".\"Id\" = @customerId";
            using (NpgsqlCommand command = new NpgsqlCommand(updateQuery, _connection))
            {
                command.Parameters.AddWithValue("@noShow", true);
                command.Parameters.AddWithValue("@customerId", Id);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task MarkAsFinished(int Id)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            CustomerTicket customer = null;
            string query = "SELECT * FROM public.\"Customers\" WHERE public.\"Customers\".\"Id\" = @customerId";
            // string query = "SELECT * FROM \"Customers\" WHERE \"Customers\".\"Id\" = @customerId";
            using (NpgsqlCommand command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@customerId", Id);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        customer = new CustomerTicket
                        {
                            Id = (int)reader["Id"],
                            Status = (string)reader["Status"],
                            Completed = (bool)reader["Completed"]
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("EndServiceTime")))
                        {
                            customer.EndServiceTime = (DateTime)reader["EndServiceTime"];
                        }
                    }
                }
            }
            // Update the customer's status to "no show" and remove them from the queue
            string updateQuery = "UPDATE public.\"Customers\" SET \"Status\" = 'Finished', \"Completed\" = @completed, \"EndServiceTime\" = @endservicetime  WHERE public.\"Customers\".\"Id\" = @customerId";
            using (NpgsqlCommand command = new NpgsqlCommand(updateQuery, _connection))
            {
                command.Parameters.AddWithValue("@completed", true);
                command.Parameters.AddWithValue("@endservicetime", DateTime.UtcNow);
                command.Parameters.AddWithValue("@customerId", Id);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<CustomerTicket> RecallNumber(int Id, int? servicePointId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            CustomerTicket customer = null;
            string query = $"SELECT * FROM public.\"Customers\" WHERE public.\"Customers\".\"Id\" = @customerId";
            using (NpgsqlCommand command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@customerId", Id);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        customer = new CustomerTicket
                        {
                            Id = (int)reader["Id"],
                            ServicePointId = (int)reader["Id"]
                        };
                    }
                }
            }

            // Create a new SpeechSynthesizer instance
            using SpeechSynthesizer synth = new SpeechSynthesizer();

            // Set the desired voice by ID
            synth.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult, 0, new System.Globalization.CultureInfo("en-US"));

            // Speak the ticket number and service point ID of the next customer
            string textToSpeak = "Ticket number " + customer.Id + ", please proceed to room " + servicePointId;
            synth.Speak(textToSpeak);
            return customer;
        }

        public async Task<TransferView> TransferNumber(int Id, int? servicePointId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            CustomerTicket customer = null;
            string query = $"SELECT * FROM public.\"Customers\" WHERE public.\"Customers\".\"Id\" = @customerId";
            using (NpgsqlCommand command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@customerId", Id);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        customer = new CustomerTicket
                        {
                            Id = (int)reader["Id"],
                            ServicePointId = (int)reader["Id"]
                        };
                    }
                }
            }
            var servicePoints = await GetAvailableServicePointsAsync(servicePointId);
            var model = new TransferView
            {
                Customers = customer,
                ServicePoints = servicePoints
            };
            return model;
        }

        private async Task<List<ServicePoint>> GetAvailableServicePointsAsync(int? servicePointId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            var servicePoints = new List<ServicePoint>();
            string query = $"SELECT * FROM public.\"ServicePoints\" WHERE public.\"ServicePoints\".\"Id\" <> @Id";
            await using (NpgsqlCommand command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@id", servicePointId);
                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var servicePoint = new ServicePoint
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            ServiceProviderId = (int)reader["ServiceProviderId"]

                        };
                        servicePoints.Add(servicePoint);
                    }
                }
            }
            return servicePoints;
        }

        public async Task TransferPost(int Id, int servicePointId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            string query = "UPDATE public.\"Customers\" SET \"ServicePointId\" = @servicePointId, \"Status\" = 'Waiting' WHERE public.\"Customers\".\"Id\" = @Id";

            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("servicePointId", servicePointId);
                command.Parameters.AddWithValue("@Id", Id);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<Models.ServiceProvider> Login(string Name, string Password)
        {
            var query = $"SELECT * FROM public.\"ServiceProviders\" WHERE public.\"ServiceProviders\".\"Name\" = @name";

            // Password = BCrypt.Net.BCrypt.Verify(Password, provider.PasswordHash)
            var parameters = new List<NpgsqlParameter>
                {
                    new NpgsqlParameter("@name", Name)
                };
            Models.ServiceProvider provider = await AuthenticateProvider(query, parameters);

            if (provider is null || !BC.Verify(Password, provider.Password))
            {
                return null;
                // TempData["error"] = "Invalid Login Credentials";
            }
            else
            {
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, provider.Password),
                        new Claim(ClaimTypes.Role, "serviceProvider")
                    };

                var claimsIdentity = new ClaimsIdentity(
                    claims, "ServicePointAuthentication");

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await _httpContextAccessor.HttpContext.SignInAsync(
                    "ServicePointAuthentication",
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return provider;
            }
        }

        public async Task<Models.ServiceProvider> AuthenticateProvider(string query, List<NpgsqlParameter> parameters)
        {
            Models.ServiceProvider provider = null;

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
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        provider = new Models.ServiceProvider
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            Password = (string)reader["Password"],
                            ServicePointId = (int)reader["ServicePointId"],
                        };
                    }
                }
            }
            return provider;
        }
    }

}