using Npgsql;
using Queue_Management_System.Contracts;
using Queue_Management_System.Data;
using Queue_Management_System.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

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

     /*   private NpgsqlConnection connection;*/
        private NpgsqlConnection connection;
        private static NpgsqlConnection connection2;

        public QueueRepository()
        {
            connection = new NpgsqlConnection(CONNECTION_STRING);
            connection2 = new NpgsqlConnection(CONNECTION_STRING);

            connection.Open();
            connection2.Open();
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
            string commandText = $"INSERT INTO {_queueTable} (servicepointid, status) VALUES (@servicepointid, {status})";


            await using (var cmd = new NpgsqlCommand(commandText, connection))
            {
                cmd.Parameters.AddWithValue("servicepointid", customer.Id);
                cmd.Parameters.AddWithValue("status", status);

                await cmd.ExecuteNonQueryAsync();
            }
        }       
        public async Task<IEnumerable<QueueVM>> GetCalledCustomers()
        {
            List<QueueVM> calledCustomers = new List<QueueVM>();

            string commandText = $"SELECT * FROM {_queueTable} WHERE status = 2";
            await using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, connection))
            await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                {
                    QueueVM calledCustomer = ReadCalledCustomers(reader);
                    calledCustomers.Add(calledCustomer);
                }

            return calledCustomers;
        }

        private static QueueVM ReadCalledCustomers(NpgsqlDataReader reader)
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
            List<QueueVM> waitingCustomers = new List<QueueVM>();
            string commandText = $"SELECT * FROM {_queueTable} WHERE servicepointid = {userServingPointId} AND status = 0 ORDER BY id ASC";
            await using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, connection))
            await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                {
                    QueueVM waitingCustomer = ReadWaitingCustomers(reader);
                    waitingCustomers.Add(waitingCustomer);
                }

            return waitingCustomers;
        }

        private static QueueVM ReadWaitingCustomers(NpgsqlDataReader reader)
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
            string commandText = $"SELECT * FROM {_queueTable} WHERE status = 2 AND servicepointid = {userServingPointId}";
            await using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, connection))
            {
                cmd.Parameters.AddWithValue("Id", userServingPointId);

                await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        QueueVM MyCurrentCustomerDetails = ReadMyCurrentServingCustomerDetails(reader);
                        return MyCurrentCustomerDetails;
                    }
            }
            return null;
        }

        private static QueueVM ReadMyCurrentServingCustomerDetails(NpgsqlDataReader reader)
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
            //Update the current customer as served
            var commandText = $@"UPDATE {_queueTable} SET status = 3 WHERE id = @id";
            await using (var cmd = new NpgsqlCommand(commandText, connection))
            {
                cmd.Parameters.AddWithValue("id", outgoingCustomerId);

                await cmd.ExecuteNonQueryAsync();
            }

            //Get id of the next customer to be served
            string commandText2 = $"SELECT * FROM {_queueTable} WHERE status = 0 AND servicepointid = {serviceProviderId} ORDER BY id ASC LIMIT 1  "; 

            await using (NpgsqlCommand cmd = new NpgsqlCommand(commandText2, connection))
            {
                await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        QueueVM IncomingCustomerDetails = ReadIncomingCustomerId(reader);
                     
                       
                            return IncomingCustomerDetails;
                                             
                    }
            }
            return null;
        }
        private static QueueVM ReadIncomingCustomerId(NpgsqlDataReader reader)

        {
            int? incomingCustomerId = reader["id"] as int?;
            QueueVM IncomingCustomerId = new QueueVM
            {
                Id = (int)incomingCustomerId
            };
            UpdateIncomingCustomerStatus(IncomingCustomerId.Id);
            return IncomingCustomerId;
        }

        private static async void UpdateIncomingCustomerStatus(int? incomingCustomerId) 
                                                         
        {
            var commandText = $@"UPDATE {_queueTable} SET status = 2, updatedat = NOW() WHERE id = @id";
            await using (var cmd = new NpgsqlCommand(commandText, connection2))
            {
                cmd.Parameters.AddWithValue("id", incomingCustomerId);

               await cmd.ExecuteNonQueryAsync();                 
            }
        }


        //
        /*    public async Task MarkNumberASFinished(int id)
            {
                //Update the current customer as served
                var commandText = $@"UPDATE {_queueTable} SET status = 3 WHERE id = @id";
                await using (var cmd = new NpgsqlCommand(commandText, connection))
                {
                    cmd.Parameters.AddWithValue("id", id);

                    await cmd.ExecuteNonQueryAsync();
                }
            }*/

        public async Task<QueueVM> MarkNumberASFinished(string serviceProviderId)
        {
            QueueVM customerIdToMarkAsFinished = await MyCurrentServingCustomer(serviceProviderId);
            //Update the current customer as served
            var commandText = $@"UPDATE {_queueTable} SET status = 3 WHERE id = @id";
            await using (var cmd = new NpgsqlCommand(commandText, connection))
            {
                cmd.Parameters.AddWithValue("id", customerIdToMarkAsFinished.Id);

                await cmd.ExecuteNonQueryAsync();
            }
            return null;
        }


    }
}
