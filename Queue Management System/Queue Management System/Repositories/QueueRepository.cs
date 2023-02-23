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
            using (NpgsqlCommand command = new NpgsqlCommand(commandText, _connection))
            {
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                        while (await reader.ReadAsync())
                        {
                            services.Add(new ServicePointVM
                            {
                                Id = (int)reader["id"],
                                Name = (string)reader["name"]
                            });
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
            int status = 0;
            string commandText = $"INSERT INTO {_queueTable} (servicepointid, status) VALUES (@servicepointid, @status)";

            using (NpgsqlCommand command = new NpgsqlCommand(commandText, _connection))
            {
                command.Parameters.AddWithValue("@servicepointid", servicePointId.Id);
                command.Parameters.AddWithValue("@status", status);

                await command.ExecuteNonQueryAsync();
            }
            _connection.Close();
        }       
        public async Task<IEnumerable<QueueVM>> GetCalledCustomers()
        {
            OpenConnection();
            List<QueueVM> calledCustomers = new List<QueueVM>();
            int status = 2;

            string commandText = $"SELECT id, servicepointid FROM {_queueTable} WHERE status = {status}";
            using (NpgsqlCommand command = new NpgsqlCommand(commandText, _connection))
            {
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        calledCustomers.Add(new QueueVM
                        {
                            Id = (int)reader["id"],
                            ServicePointId = (int)reader["servicepointid"]
                        });
                    }
                        reader.Close();                  
                }
                _connection.Close();
            }
            if (calledCustomers.Count() == 0)
            {
                return null;
            }
            return calledCustomers;
        }
        public async Task<IEnumerable<QueueVM>> GetWaitingCustomers(int servicePointId)
        {
            OpenConnection();
            List<QueueVM> waitingCustomers = new List<QueueVM>();
            int status = 0;

            string commandText = $"SELECT id, createdat " +
                                 $"FROM {_queueTable} WHERE servicepointid = {servicePointId} " +
                                 $"AND status = {status} ORDER BY id ASC";
            using (NpgsqlCommand command = new NpgsqlCommand(commandText, _connection))
            {
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        waitingCustomers.Add(new QueueVM
                        {
                            Id = (int)reader["id"],
                            CreatedAt = (DateTime)reader["createdat"]
                        });
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
            int status = 2;

            string commandText = $"SELECT id FROM {_queueTable} " +
                                 $"WHERE status = {status} AND servicepointid = {servicePointId}";
            using (NpgsqlCommand command = new NpgsqlCommand(commandText, _connection))
            {
                command.Parameters.AddWithValue("Id", servicePointId);

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        myCurrentCustomerId = new QueueVM
                        {
                            Id = (int)reader["id"]
                        };
                    }
                    reader.Close();
                }
                _connection.Close();              
            }
            if (myCurrentCustomerId == null)
            {
                return null;
            }
            return myCurrentCustomerId;
        }
        public async Task<QueueVM> UpdateOutGoingAndIncomingCustomerStatus(int outgoingCustomerId, int servicePointId)
        {           
            //if there is no customer being served, no need to update status
            if (outgoingCustomerId != 0)
            {
                OpenConnection();
                int status = 3;
                //Update the current customer as served
                string commandText = $"UPDATE {_queueTable} SET status = {status}, completedat = NOW() WHERE id = @id";
                using (NpgsqlCommand command = new NpgsqlCommand(commandText, _connection))
                {
                    command.Parameters.AddWithValue("@id", outgoingCustomerId);

                    await command.ExecuteNonQueryAsync();
                }
                _connection.Close();
            }
            QueueVM nextCustomerId = await GetNextCustomerId(servicePointId);
            if(nextCustomerId != null)
            {
                return nextCustomerId;
            }
            return null;
        }
        private async Task<QueueVM> GetNextCustomerId(int servicePointId)
        {
            OpenConnection();
            QueueVM incomingCustomerId = null;
            int status = 0;

            string commandText = $"SELECT id FROM {_queueTable} WHERE status = {status} " +
                                $"AND servicepointid = {servicePointId} ORDER BY id ASC LIMIT 1  ";
            using (NpgsqlCommand command = new NpgsqlCommand(commandText, _connection))
            {
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
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
            int status = 2;

            string commandText = $"UPDATE {_queueTable} SET status = {status}, updatedat = NOW() WHERE id = @id";
            using (NpgsqlCommand command = new NpgsqlCommand(commandText, _connection))
            {
                command.Parameters.AddWithValue("@id", incomingCustomerId);

                await command.ExecuteNonQueryAsync();                 
            }
            _connection.Close();
        }
        public async Task MarkNumberASNoShow(int servicePointId)
        {
            QueueVM customerIdToMarkAsFinished = await MyCurrentServingCustomer(servicePointId); 

            OpenConnection();
            int status = 4;

            string commandText = $"UPDATE {_queueTable} SET status = {status} , " +
                                 $"updatedat= NULL, completedat = NULL WHERE id = @id";
            using (NpgsqlCommand command = new NpgsqlCommand(commandText, _connection))
            {
                command.Parameters.AddWithValue("@id", customerIdToMarkAsFinished.Id);

                await command.ExecuteNonQueryAsync();
            }
            _connection.Close();
        }
        public async Task MarkNumberASFinished(int servicePointId)
        {
            QueueVM customerIdToMarkAsFinished = await MyCurrentServingCustomer(servicePointId);

            OpenConnection();
            int status = 3;

            string commandText = $"UPDATE {_queueTable} SET status = {status} , completedat = NOW() WHERE id = @id";
            using (NpgsqlCommand command = new NpgsqlCommand(commandText, _connection))
            {
                command.Parameters.AddWithValue("@id", customerIdToMarkAsFinished.Id);

                await command.ExecuteNonQueryAsync();
            }
            _connection.Close();
        }
        public async Task TransferNumber(int servicePointId, int servicePointIdTranser)
        {
            QueueVM customerIdToMarkAsFinished = await MyCurrentServingCustomer(servicePointId);

            OpenConnection();
            int status = 0;
            string commandText = $"UPDATE {_queueTable} SET servicepointid = {servicePointIdTranser}, " +
                                 $"status = {status}, updatedat= NULL, completedat = NULL WHERE id = @id";
            using (NpgsqlCommand command = new NpgsqlCommand(commandText, _connection))
            {
                command.Parameters.AddWithValue("@id", customerIdToMarkAsFinished.Id);

                await command.ExecuteNonQueryAsync();
            }
            _connection.Close();
        }
    }
}
