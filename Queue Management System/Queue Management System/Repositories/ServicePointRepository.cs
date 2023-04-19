using Microsoft.AspNetCore.Http;
using Npgsql;
using Queue_Management_System.Models;
using Queue_Management_System.Services;

namespace Queue_Management_System.Repositories
{
    public class ServicePointRepository : IServicePointRepository
    {
        private readonly string _connectionString;
        private IConfiguration _config;
        private NpgsqlConnection _connection;
        public ServicePointRepository(IConfiguration config)
        {
            _config = config;
        }
        private void OpenConnection()
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");

            _connection = new NpgsqlConnection(connectionString);
            _connection.Open();
        }

        public async Task<List<CustomerTicket>> GetQueueStatus(int? servicePointId)
        {
            OpenConnection();
            var customers = new List<CustomerTicket>();
            string query = $"SELECT * FROM public.\"Customers\" WHERE public.\"Customers\".\"ServicePointId\" = @ServicePointId AND NOT public.\"Customers\".\"Completed\"";
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
                    reader.Close();
                }
            }
            if (customers.Count() == 0)
                return new List<CustomerTicket>();
            return customers;
        }

        public async Task<CustomerTicket> GetNextNumber(int? servicePointId)
        {
            OpenConnection();

            // Query the database to retrieve the next customer in the queue with the selected service point ID and status "waiting"
            CustomerTicket nextCustomer = null;
            string query = "SELECT * FROM public.\"Customers\" WHERE public.\"Customers\".\"ServicePointId\" = @ServicePointId AND public.\"Customers\".\"Status\" = 'Waiting'";
            using (NpgsqlCommand command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@servicePointId", servicePointId);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        nextCustomer = new CustomerTicket
                        {
                            Id = (int)reader["Id"],
                            CheckInTime = (DateTime)reader["CheckInTime"],
                            Status = (string)reader["Status"]
                        };
                    }
                    reader.Close();
                }
            }

            // If there is no next customer, display a message to the service provider
            if (nextCustomer == null)
            {
                return null;
            }

            // Update the customer's status to "in progress"
            string updateQuery = "UPDATE public.\"Customers\" SET \"Status\" = 'In Progress', \"StartServiceTime\" = @startServiceTime, \"CallTime\" = @callTime, \"IsCalled\" = @isCalled WHERE public.\"Customers\".\"Id\" = @customerId";
            using (NpgsqlCommand command = new NpgsqlCommand(updateQuery, _connection))
            {
                command.Parameters.AddWithValue("@startServiceTime", DateTime.UtcNow.Add(DateTime.Now.TimeOfDay));
                command.Parameters.AddWithValue("@callTime", DateTime.UtcNow);
                command.Parameters.AddWithValue("@isCalled", true);
                command.Parameters.AddWithValue("@customerId", nextCustomer.Id);
                await command.ExecuteNonQueryAsync();
            }
            // await Task.CompletedTask;
            return nextCustomer;
        }

        public async Task MarkAsNoShow(int Id, int? servicePointId)
        {
            OpenConnection();

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
                    reader.Close();
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
    }
}