using Npgsql;
using Queue_Management_System.Services;
using Queue_Management_System.Models;
using System.Data;
using System.Net;

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
        public async Task<IEnumerable<ServicePointVM>> GetServices()
        {
            OpenConnection();
            List<ServicePointVM> services = new List<ServicePointVM>();

            string commandText = $"SELECT id, name FROM {_servicePointsTable}";
            using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, _connection))
            {
                using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                        while (await reader.ReadAsync())
                        {
                            ServicePointVM service = new ServicePointVM
                            {
                                Id = (int)reader["id"],
                                Name = (string)reader["name"]
                            };
                            services.Add(service);
                        }
                        reader.Close();
                }
                _connection.Close();
            }
            if (services.Count() == 0)
            {
                return null;
            }
            return services;
        }
        public async Task AddCustomerToQueue(ServicePointVM servicePointId)
        {
            OpenConnection();
            var status = 0;
            string commandText = $"INSERT INTO {_queueTable} (servicepointid, status) VALUES (@servicepointid, {status})";

            using (var cmd = new NpgsqlCommand(commandText, _connection))
            {
                cmd.Parameters.AddWithValue("servicepointid", servicePointId.Id);
                cmd.Parameters.AddWithValue("status", status);

                await cmd.ExecuteNonQueryAsync();
            }
            _connection.Close();
        }       
        public async Task<IEnumerable<QueueVM>> GetCalledCustomers()
        {
            OpenConnection();
            List<QueueVM> calledCustomers = new List<QueueVM>();

            string commandText = $"SELECT id, servicepointid FROM {_queueTable} WHERE status = 2";
            using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, _connection))
            {
                using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                        while (await reader.ReadAsync())
                        {
                            QueueVM calledCustomer = new QueueVM
                            {
                                Id = (int)reader["id"],
                                ServicePointId = (int)reader["servicepointid"]
                            };
                            calledCustomers.Add(calledCustomer);
                        }
                        reader.Close();                  
                }
                _connection.Close();
            }          
            return calledCustomers;
        }
        public async Task<IEnumerable<QueueVM>> GetWaitingCustomers(int servicePointId)
        {
            OpenConnection();
            List<QueueVM> waitingCustomers = new List<QueueVM>();

            string commandText = $"SELECT id, createdat FROM {_queueTable} WHERE servicepointid = {servicePointId} AND status = 0 ORDER BY id ASC";
            using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, _connection))
            {
                using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                        while (await reader.ReadAsync())
                        {
                            QueueVM waitingCustomer = new QueueVM
                            {
                                Id = (int)reader["id"],
                                CreatedAt = (DateTime)reader["createdat"]
                            };
                            waitingCustomers.Add(waitingCustomer);
                        }
                        reader.Close();                
                }
                _connection.Close();
            }
            if (waitingCustomers.Count() == 0)
            {
                return null;
            }
            return waitingCustomers;
        }    
        public async Task<QueueVM> MyCurrentServingCustomer(int servicePointId)
        {
            OpenConnection();

            QueueVM myCurrentCustomerId = null;

            string commandText = $"SELECT id FROM {_queueTable} WHERE status = 2 AND servicepointid = {servicePointId}";
            using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, _connection))
            {
                cmd.Parameters.AddWithValue("Id", servicePointId);

                using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        myCurrentCustomerId = new QueueVM
                        {
                            Id = (int?)reader["id"]
                        };
                    }
                    reader.Close();
                }
                _connection.Close();
                if (myCurrentCustomerId == null)
                {
                    return null;
                }
                return myCurrentCustomerId;
            }
        }
        public async Task<QueueVM> UpdateOutGoingAndIncomingCustomerStatus(int outgoingCustomerId, int servicePointId)
        {           
            //if there is no customer being served, no need to update status
            if (outgoingCustomerId != 0)
            {
                OpenConnection();
                //Update the current customer as served
                var commandText = $@"UPDATE {_queueTable} SET status = 3, completedat = NOW() WHERE id = @id";
                using (var cmd = new NpgsqlCommand(commandText, _connection))
                {
                    cmd.Parameters.AddWithValue("id", outgoingCustomerId);

                    await cmd.ExecuteNonQueryAsync();
                }
                _connection.Close();
            }
            QueueVM nextCustomerId = await GetIdOfNextCustomer(servicePointId);
            if(nextCustomerId != null)
            {
                return nextCustomerId;
            }
            return null;
        }

        //Get id of the next customer to be served
        private async Task<QueueVM> GetIdOfNextCustomer(int servicePointId)
        {
            OpenConnection();
            QueueVM incomingCustomerId = null;

            string commandText = $"SELECT id FROM {_queueTable} WHERE status = 0 AND servicepointid = {servicePointId} ORDER BY id ASC LIMIT 1  ";

            using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, _connection))
            {
                using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        incomingCustomerId = new QueueVM
                        {
                            Id = (int)reader["id"]
                        };
                    }
                    reader.Close();
                }
                _connection.Close();
                if (incomingCustomerId != null)
                {
                    UpdateIncomingCustomerStatus(incomingCustomerId.Id);
                }
                return incomingCustomerId;
            }
            return null;
        }
        private  async Task UpdateIncomingCustomerStatus(int? incomingCustomerId)                                                          
        {
            OpenConnection();
            var commandText = $@"UPDATE {_queueTable} SET status = 2, updatedat = NOW() WHERE id = @id";
            using (var cmd = new NpgsqlCommand(commandText, _connection))
            {
                cmd.Parameters.AddWithValue("id", incomingCustomerId);

                await cmd.ExecuteNonQueryAsync();                 
            }
            _connection.Close();
        }
        public async Task MarkNumberASNoShow(int servicePointId)
        {
            QueueVM customerIdToMarkAsFinished = await MyCurrentServingCustomer(servicePointId); 

            OpenConnection();

            //Update the current customer as served
            var commandText = $@"UPDATE {_queueTable} SET status = 4 , updatedat= NULL, completedat = NULL WHERE id = @id";
            using (var cmd = new NpgsqlCommand(commandText, _connection))
            {
                cmd.Parameters.AddWithValue("id", customerIdToMarkAsFinished.Id);

                await cmd.ExecuteNonQueryAsync();
            }
            _connection.Close();
        }
        public async Task MarkNumberASFinished(int servicePointId)
        {
            QueueVM customerIdToMarkAsFinished = await MyCurrentServingCustomer(servicePointId);

            OpenConnection();
            //Update the current customer as served
            var commandText = $@"UPDATE {_queueTable} SET status = 3 , completedat = NOW() WHERE id = @id";
            using (var cmd = new NpgsqlCommand(commandText, _connection))
            {
                cmd.Parameters.AddWithValue("id", customerIdToMarkAsFinished.Id);

                await cmd.ExecuteNonQueryAsync();
            }
            _connection.Close();
        }
        public async Task TransferNumber(int servicePointId, int servicePointid)
        {
            QueueVM customerIdToMarkAsFinished = await MyCurrentServingCustomer(servicePointId);

            OpenConnection();
            //Update the current customer as served
            var commandText = $@"UPDATE {_queueTable} SET servicepointid = {servicePointid}, status = 0, updatedat= NULL, completedat = NULL WHERE id = @id";
            using (var cmd = new NpgsqlCommand(commandText, _connection))
            {
                cmd.Parameters.AddWithValue("id", customerIdToMarkAsFinished.Id);

                await cmd.ExecuteNonQueryAsync();
            }
            _connection.Close();
        }
    }
}
