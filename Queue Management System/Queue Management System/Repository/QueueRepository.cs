using Npgsql;
using Queue_Management_System.Models;
using Queue_Management_System.Services;
using System.Data.Common;
using System.Net;

namespace Queue_Management_System.Repository
{
    public class QueueRepository : IQueueRepository
    {
        private const string _servicePointsTable = "ServicePointsTb";
        private const string _queueTable = "QueueTb";
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
        public async Task<IEnumerable<ServicePointM>> GetServices()
        {
            OpenConnection();
            List<ServicePointM> services = new List<ServicePointM>();
            string commandText = $"SELECT id, name FROM {_servicePointsTable}";
            using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, _connection))
            {
                using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        ServicePointM service = new ServicePointM
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
        public async Task AddCustomerToQueue(ServicePointM servicePointId)
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
        public async Task<IEnumerable<QueueM>> GetCalledCustomers()
        {
            OpenConnection();
            List<QueueM> calledCustomers = new List<QueueM>();
            string commandText = $"SELECT id, servicepointid FROM {_queueTable} WHERE status = 2";
            using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, _connection))
            {
                using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        QueueM calledCustomer = new QueueM
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

            if (calledCustomers.Count() == 0)
            {
                return null;
            }
            return calledCustomers;
        }

        public async Task<IEnumerable<QueueM>> GetWaitingCustomers(int servicePointId)
        {
            OpenConnection();
            List<QueueM> waitingCustomers = new List<QueueM>();
            string commandText = $"SELECT id, createdat FROM {_queueTable} WHERE servicepointid = {servicePointId} AND status = 0 ORDER BY id ASC";
            using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, _connection))
            {
                using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        QueueM waitingCustomer = new QueueM
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
        public async Task<QueueM> CurrentServingCustomer(int servicePointId)
        {
            OpenConnection();
            QueueM myCurrentCustomerId = null;
            string commandText = $"SELECT id FROM {_queueTable} WHERE status = 2 AND servicepointid = {servicePointId}";
            using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, _connection))
            {
                cmd.Parameters.AddWithValue("Id", servicePointId);
                using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        myCurrentCustomerId = new QueueM
                        {
                            Id = (int?)reader["id"]
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
        public async Task<QueueM> UpdateOutGoingAndIncomingCustomerStatus(int outgoingCustomerId, int servicePointId)
        {
            //if there is no customer being served, no need to update status
            if (outgoingCustomerId != 0)
            {
                OpenConnection();
                //Update the current customer as served

                string commandText = $@"UPDATE {_queueTable} SET status = 3, completedat = NOW() WHERE id = @id";
                using (var cmd = new NpgsqlCommand(commandText, _connection))
                {
                    cmd.Parameters.AddWithValue("id", outgoingCustomerId);
                    await cmd.ExecuteNonQueryAsync();
                }
                _connection.Close();
            }
            QueueM nextCustomerId = await GetNextCustomerId(servicePointId);
            if (nextCustomerId != null)
            {
                return nextCustomerId;
            }
            return null;
        }
        private async Task<QueueM> GetNextCustomerId(int servicePointId)
        {
            OpenConnection();
            QueueM incomingCustomerId = null;
            string commandText = $"SELECT id FROM {_queueTable} WHERE status = 0 AND servicepointid = {servicePointId} ORDER BY id ASC LIMIT 1  ";
            using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, _connection))
            {
                using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        incomingCustomerId = new QueueM
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
        private async Task UpdateIncomingCustomerStatus(int? incomingCustomerId)
        {
            OpenConnection();

            string commandText = $@"UPDATE {_queueTable} SET status = 2, updatedat = NOW() WHERE id = @id";
            using (var cmd = new NpgsqlCommand(commandText, _connection))
            {
                cmd.Parameters.AddWithValue("id", incomingCustomerId);
                await cmd.ExecuteNonQueryAsync();
            }
            _connection.Close();
        }
        public async Task MarkNumberASNoShow(int servicePointId)
        {
            QueueM customerIdToMarkAsFinished = await CurrentServingCustomer(servicePointId);
            OpenConnection();

            //Update the current customer as served            
            string commandText = $@"UPDATE {_queueTable} SET status = 4 , updatedat= NULL, completedat = NULL WHERE id = @id";
            using (var cmd = new NpgsqlCommand(commandText, _connection))
            {
                cmd.Parameters.AddWithValue("id", customerIdToMarkAsFinished.Id);
                await cmd.ExecuteNonQueryAsync();
            }
            _connection.Close();
        }
        public async Task MarkNumberASFinished(int servicePointId)
        {
            QueueM customerIdToMarkAsFinished = await CurrentServingCustomer(servicePointId);

            OpenConnection();
            //Update the current customer as served            
            string commandText = $@"UPDATE {_queueTable} SET status = 3 , completedat = NOW() WHERE id = @id";
            using (var cmd = new NpgsqlCommand(commandText, _connection))
            {
                cmd.Parameters.AddWithValue("id", customerIdToMarkAsFinished.Id);
                await cmd.ExecuteNonQueryAsync();
            }
            _connection.Close();
        }
        public async Task TransferNumber(int servicePointId, int servicePointid)
        {
            QueueM customerIdToMarkAsFinished = await CurrentServingCustomer(servicePointId);

            OpenConnection();
            //Update the current customer as served 
            string commandText = $@"UPDATE {_queueTable} SET servicepointid = {servicePointid}, status = 0, updatedat= NULL, completedat = NULL WHERE id = @id";
            using (var cmd = new NpgsqlCommand(commandText, _connection))
            {
                cmd.Parameters.AddWithValue("id", customerIdToMarkAsFinished.Id);
                await cmd.ExecuteNonQueryAsync();
            }
            _connection.Close();
        }
        //
        Task<QueueM> IQueueRepository.MarkNumberASNoShow(int servicePointId)
        {
            throw new NotImplementedException();
        }

        Task<QueueM> IQueueRepository.MarkNumberASFinished(int servicePointId)
        {
            throw new NotImplementedException();
        }

        public Task<QueueM> TransferNumber(string serviceProviderId, int servicePointid)
        {
            throw new NotImplementedException();
        }
    }
}


