using Npgsql;
using Queue_Management_System.Contracts;
using Queue_Management_System.Models;

namespace Queue_Management_System.Repositories
{
    public class QueueRepository : IQueueRepository
    {
        private const string CONNECTION_STRING = "Host=localhost:5432;" +
                          "Username=postgres;" +
                          "Password=*mikemathu;" +
                          "Database=QMS";

        private const string _servicePointsTable = "servicepoints";
        private const string _queueTable = "queue";

        private NpgsqlConnection connection;

        public QueueRepository()
        {
            connection = new NpgsqlConnection(CONNECTION_STRING);
            connection.Open();
        }

        public async Task<IEnumerable<ServicePointVM>> GetServices()
        {
            List<ServicePointVM> services = new List<ServicePointVM>();

            string commandText = $"SELECT * FROM {_servicePointsTable}";
            await using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, connection))
            await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                {
                    ServicePointVM service = ReadServices(reader);
                    services.Add(service);
                }

            return services;
        }

        private static ServicePointVM ReadServices(NpgsqlDataReader reader)
        {
            int? id = reader["id"] as int?;
            string name = reader["name"] as string;
            int? serviceproviderId = reader["serviceproviderId"] as int?;
            string servicename = reader["servicename"] as string;
            ServicePointVM service = new ServicePointVM
            {
                Id = (int)id,
                Name = name,
                ServiceProviderId = (int)serviceproviderId,
                ServiceName = servicename,
            };
            return service;
        }

        public async Task AddCustomerToQueue(ServicePointVM customer)
        {
            var status = 0;
            /*string commandText = $"INSERT INTO {_queueTable} (servicepointid, status, createdat) VALUES (@servicepointid, {status}, {createdAt})";*/
            string commandText = $"INSERT INTO {_queueTable} (servicepointid, status) VALUES (@servicepointid, {status})";


            await using (var cmd = new NpgsqlCommand(commandText, connection))
            {
                cmd.Parameters.AddWithValue("servicepointid", customer.Id);
                cmd.Parameters.AddWithValue("status", status);

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
