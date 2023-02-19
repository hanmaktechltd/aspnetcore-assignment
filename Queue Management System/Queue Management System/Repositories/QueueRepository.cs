using Npgsql;
using Queue_Management_System.Services;
using Queue_Management_System.Models;
using System.Data;

namespace Queue_Management_System.Repositories
{
    public class QueueRepository : IQueueRepository
    {

        private const string _servicePointsTable = "servicepoints";
        private const string _queueTable = "queue";
        private IConfiguration _config;
        private NpgsqlConnection _connection;

        public QueueRepository(IConfiguration config)
        {
            _config = config;
        }
        private void OpenConnection()
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");

            _connection = new NpgsqlConnection(connectionString);
            _connection.Open();
        }

        private void CloseConnection()
        {
            if (_connection?.State != ConnectionState.Closed)
            {
                _connection?.Close();
            }
        }
        public async Task<IEnumerable<ServicePointVM>> GetServices()
        {
            OpenConnection();
            List<ServicePointVM> services = new List<ServicePointVM>();

            string commandText = $"SELECT * FROM {_servicePointsTable}";
            await using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, _connection))
            {
                await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        ServicePointVM service = ReadServices(reader);
                        services.Add(service);
                    }
                    reader.Close();
                }
                CloseConnection();
            }            
            return services;
        }
        private ServicePointVM ReadServices(NpgsqlDataReader reader)
        {
            int? id = reader["id"] as int?;
            string name = reader["name"] as string;
            int? serviceproviderId = reader["serviceproviderId"] as int?;
            ServicePointVM service = new ServicePointVM
            {
                Id = (int)id,
                Name = name,
                ServiceProviderId = (int)serviceproviderId,
            };
            return service;
        }
        public async Task AddCustomerToQueue(ServicePointVM customer)
        {
            OpenConnection();
            var status = 0;
            string commandText = $"INSERT INTO {_queueTable} (servicepointid, status) VALUES (@servicepointid, {status})";

            await using (var cmd = new NpgsqlCommand(commandText, _connection))
            {
                cmd.Parameters.AddWithValue("servicepointid", customer.Id);
                cmd.Parameters.AddWithValue("status", status);

                await cmd.ExecuteNonQueryAsync();
            }
            CloseConnection();
        }       
        public async Task<IEnumerable<QueueVM>> GetCalledCustomers()
        {
            OpenConnection();
            List<QueueVM> calledCustomers = new List<QueueVM>();

            string commandText = $"SELECT * FROM {_queueTable} WHERE status = 2";
            await using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, _connection))
            {
                await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        QueueVM calledCustomer = ReadCalledCustomers(reader);
                        calledCustomers.Add(calledCustomer);
                    }
                    reader.Close();
                }
                CloseConnection();
            }          
            return calledCustomers;
        }
        private QueueVM ReadCalledCustomers(NpgsqlDataReader reader)
        {
            int? calledCustomerId = reader["id"] as int?;
            int? servicePointId = reader["servicepointid"] as int?;
            QueueVM calledCustomer = new QueueVM
            {
                Id = (int)calledCustomerId,
                ServicePointId = (int)servicePointId
            };
            return calledCustomer;
        }
        public async Task<IEnumerable<QueueVM>> GetWaitingCustomers(string userServingPointId)
        {
            OpenConnection();
            List<QueueVM> waitingCustomers = new List<QueueVM>();

            string commandText = $"SELECT * FROM {_queueTable} WHERE servicepointid = {userServingPointId} AND status = 0 ORDER BY id ASC";
            await using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, _connection))
            {
                await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        QueueVM waitingCustomer = ReadWaitingCustomers(reader);
                        waitingCustomers.Add(waitingCustomer);
                    }
                    reader.Close();
                }
                CloseConnection();
            }           
            return waitingCustomers;
        }
        private QueueVM ReadWaitingCustomers(NpgsqlDataReader reader)
        {
            int? id = reader["id"] as int?;
            DateTime createdat = (DateTime)reader["createdat"];
            QueueVM waitingCustomers = new QueueVM
            {
                Id = (int)id,
                CreatedAt = createdat
            };
            return waitingCustomers;
        }    
        public async Task<QueueVM> MyCurrentServingCustomer(string userServingPointId)
        {
            OpenConnection();
            string commandText = $"SELECT * FROM {_queueTable} WHERE status = 2 AND servicepointid = {userServingPointId}";
            await using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, _connection))
            {
                cmd.Parameters.AddWithValue("Id", userServingPointId);

                await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        QueueVM MyCurrentCustomerDetails = ReadMyCurrentServingCustomerDetails(reader);
                        return MyCurrentCustomerDetails;
                    }
                    reader.Close();
                }
                CloseConnection();                   
            }
            return null;
        }
        private QueueVM ReadMyCurrentServingCustomerDetails(NpgsqlDataReader reader)
        {
            int? myCurrentCustomerId = reader["id"] as int?;
            QueueVM MyCurrentCustomerDetails = new QueueVM
            {
                Id = (int)myCurrentCustomerId
            };
            return MyCurrentCustomerDetails;
        }
        public async Task<QueueVM> UpdateOutGoingAndIncomingCustomerStatus(int outgoingCustomerId, string serviceProviderId)
        {
            OpenConnection();
            //Update the current customer as served
            var commandText = $@"UPDATE {_queueTable} SET status = 3, completedat = NOW() WHERE id = @id";
            await using (var cmd = new NpgsqlCommand(commandText, _connection))
            {
                cmd.Parameters.AddWithValue("id", outgoingCustomerId);

                await cmd.ExecuteNonQueryAsync();
            }

            //Get id of the next customer to be served
            string commandText2 = $"SELECT * FROM {_queueTable} WHERE status = 0 AND servicepointid = {serviceProviderId} ORDER BY id ASC LIMIT 1  "; 

            await using (NpgsqlCommand cmd = new NpgsqlCommand(commandText2, _connection))
            {
                await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        QueueVM IncomingCustomerDetails = ReadIncomingCustomerId(reader);
                        return IncomingCustomerDetails;
                    }
                    reader.Close();
                }                   
                CloseConnection();
            }
            return null;
        }
        private QueueVM ReadIncomingCustomerId(NpgsqlDataReader reader)
        {
            int? incomingCustomerId = reader["id"] as int?;
            QueueVM IncomingCustomerId = new QueueVM
            {
                Id = (int)incomingCustomerId
            };
            UpdateIncomingCustomerStatus(IncomingCustomerId.Id);
            return IncomingCustomerId;
        }
        private  async void UpdateIncomingCustomerStatus(int? incomingCustomerId)                                                          
        {
            OpenConnection();
            var commandText = $@"UPDATE {_queueTable} SET status = 2, updatedat = NOW() WHERE id = @id";
            await using (var cmd = new NpgsqlCommand(commandText, _connection))
            {
                cmd.Parameters.AddWithValue("id", incomingCustomerId);

                await cmd.ExecuteNonQueryAsync();                 
            }
            CloseConnection();
        }
        public async Task<QueueVM> GetCurentlyCalledNumber(string serviceProviderId)
        {
            OpenConnection();
            //Get id of the next customer to be served
            string commandText2 = $"SELECT * FROM {_queueTable} WHERE status = 2 AND servicepointid = {serviceProviderId}  ";

            await using (NpgsqlCommand cmd = new NpgsqlCommand(commandText2, _connection))
            {
                await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        QueueVM CurrentlyCalledCustomerDetails = ReadCurrentlyCalledNumber(reader);
                        return CurrentlyCalledCustomerDetails;
                    }
                    reader.Close();
                }
                CloseConnection();
            }
            return null;
        }
        private QueueVM ReadCurrentlyCalledNumber(NpgsqlDataReader reader)
        {
            int? currentlyCalledCustomerId = reader["id"] as int?;
            QueueVM CurrentlyCalledCustomerId = new QueueVM 
            {
                Id = (int)currentlyCalledCustomerId
            };
            return CurrentlyCalledCustomerId;
        }
        public async Task<QueueVM> MarkNumberASNoShow(string serviceProviderId)
        {
            QueueVM customerIdToMarkAsFinished = await MyCurrentServingCustomer(serviceProviderId);

            OpenConnection();

            //Update the current customer as served
            var commandText = $@"UPDATE {_queueTable} SET status = 4 , updatedat= NULL, completedat = NULL WHERE id = @id";
            await using (var cmd = new NpgsqlCommand(commandText, _connection))
            {
                cmd.Parameters.AddWithValue("id", customerIdToMarkAsFinished.Id);

                await cmd.ExecuteNonQueryAsync();
            }
            CloseConnection();
            return null;
        }
        public async Task<QueueVM> MarkNumberASFinished(string serviceProviderId)
        {
            QueueVM customerIdToMarkAsFinished = await MyCurrentServingCustomer(serviceProviderId);

            OpenConnection();
            //Update the current customer as served
            var commandText = $@"UPDATE {_queueTable} SET status = 3 , completedat = NOW() WHERE id = @id";
            await using (var cmd = new NpgsqlCommand(commandText, _connection))
            {
                cmd.Parameters.AddWithValue("id", customerIdToMarkAsFinished.Id);

                await cmd.ExecuteNonQueryAsync();
            }
            CloseConnection();
            return null;
        }
        public async Task<QueueVM> TransferNumber(string serviceProviderId, int servicePointid)
        {
            QueueVM customerIdToMarkAsFinished = await MyCurrentServingCustomer(serviceProviderId);

            OpenConnection();
            //Update the current customer as served
            var commandText = $@"UPDATE {_queueTable} SET servicepointid = {servicePointid}, status = 0, updatedat= NULL, completedat = NULL WHERE id = @id";
            await using (var cmd = new NpgsqlCommand(commandText, _connection))
            {
                cmd.Parameters.AddWithValue("id", customerIdToMarkAsFinished.Id);

                await cmd.ExecuteNonQueryAsync();
            }
            CloseConnection();
            return null;
        }
    }
}
