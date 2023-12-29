﻿using Npgsql;
using Queue_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Queue_Management_System.Repository
{
    public class DbOperationsRepository
    {
        private readonly NpgsqlConnectionFactory _connectionFactory;


        public DbOperationsRepository(NpgsqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<List<ServiceTypeModel>> GetAvailableServicesAsync()
        {
            var serviceTypes = new List<ServiceTypeModel>();

            try
            {
                // Open a connection using the connection factory
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                var query = "SELECT Id, Name, Description FROM ServiceTypes";

                using var cmd = new NpgsqlCommand(query, connection);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var serviceType = new ServiceTypeModel
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2)
                    };

                    serviceTypes.Add(serviceType);
                }
            }
            catch (Exception ex)
            {
                // Handle exception or log the error
                Console.WriteLine($"Error fetching services: {ex.Message}");
            }

            return serviceTypes;
        }

        public async Task<bool> SaveSelectedService(string ticketNumber, string serviceName, string customerName, int servicePointId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                // Construct the SQL query with positional parameters
                var query = "INSERT INTO QueueEntry (TicketNumber, ServicePoint, CustomerName, CheckinTime, ServicePointId) " +
                            "VALUES (@ticketNumber, @servicePoint, @customerName, @checkinTime, @servicePointId)";

                using var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("ticketNumber", ticketNumber);
                cmd.Parameters.AddWithValue("servicePoint", serviceName);
                cmd.Parameters.AddWithValue("customerName", customerName);
                cmd.Parameters.AddWithValue("checkinTime", DateTime.Now);
                cmd.Parameters.AddWithValue("ServicePointId", servicePointId);

                // Execute the SQL command to insert the selected service into the table
                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                // Return true if the insertion was successful (at least one row affected)
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Handle exception or log the error
                Console.WriteLine($"Error saving selected service: {ex.Message}");
                return false;
            }
        }


        public async Task<WaitingPageModel> GetTopQueueEntryAsync()
        {
            WaitingPageModel waiting = null;

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                var query = "SELECT id, ticketnumber, servicepoint, customername, checkintime " +
                            "FROM public.queueentry " +
                            "ORDER BY id ASC " +
                            "LIMIT 1";

                using var cmd = new NpgsqlCommand(query, connection);
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var ticketNumber = reader.GetString(1);
                    var servicePoint = reader.GetString(2);
                    var customerName = reader.GetString(3);
                    var checkinTime = reader.GetDateTime(4);

                    waiting = new WaitingPageModel(ticketNumber, servicePoint, customerName, checkinTime);
                }

            }
            catch (Exception ex)
            {
                // Handle exception or log the error
                Console.WriteLine($"Error fetching top queue entry: {ex.Message}");
            }

            return waiting;
        }

        public async Task<ServiceProviderModel> LoginAsync(string usernameOrEmail, string password)
        {
            ServiceProviderModel serviceProvider = null;

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                
                var query = "SELECT Id, Username, Email, Phone, PasswordHash, RegistrationDate, ServicePoint, ServiceTypeId, isauthorized   FROM serviceProviders" +
                    $"  WHERE(email = '{usernameOrEmail}' or username = '{usernameOrEmail}')   AND PasswordHash = '{password}'";

                using var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("UsernameOrEmail", usernameOrEmail);
                cmd.Parameters.AddWithValue("Password", password);

                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    serviceProvider = new ServiceProviderModel
                    {
                        Id = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Email = reader.GetString(2),
                        Phone = reader.GetString(3),
                        PasswordHash = reader.GetString(4),
                        RegistrationDate = reader.GetDateTime(5),
                        ServicePoint = reader.GetString(6),
                        ServiceTypeId = reader.GetInt32(7),
                        IsAuthorized = reader.GetBoolean(8)


                    };
                }
            }
            catch (Exception ex)
            {
                // Handle exception or log the 
                Console.WriteLine($"Error during login: {ex.Message}");
            }

            return serviceProvider;
        }

        public async Task<QueueEntry> GetLatestQueueEntryAsync(string servicepoint)
        {
            QueueEntry latestEntry = null;

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                var query = "SELECT id, ticketnumber, servicepoint, customername, checkintime" +
                    " FROM public.queueentry WHERE servicepoint= '" + servicepoint + "' AND recallcount <3 AND noshow = 0 " +
                    "and markfinished = 0 ORDER BY id ASC LIMIT 1";

                using var cmd = new NpgsqlCommand(query, connection);
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    latestEntry = new QueueEntry
                    {
                        Id = reader.GetInt32(0),
                        TicketNumber = reader.GetString(1),
                        ServicePoint = reader.GetString(2),
                        CustomerName = reader.GetString(3),
                        CheckinTime = reader.GetDateTime(4)
                    };
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine($"Error retrieving latest queue entry: {ex.Message}");
            }

            return latestEntry;
        }

        public async Task<bool> InsertIntoFinishedTableAsync(string ticketNumber, int servicePointId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                var query = "INSERT INTO public.finishedtable(ticketnumber, servicepointId) " +
                            "VALUES (@ticketNumber, @servicePointId)";

                using var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ticketNumber", ticketNumber);
                cmd.Parameters.AddWithValue("@servicePointId", servicePointId);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                // Return true if the insertion was successful (at least one row affected)
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine($"Error inserting into finishedtable: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateQueueEntryMarkFinishedAsync(string ticketNumber)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                var query = "UPDATE public.queueentry " +
                            "SET markfinished = 1 " +
                            "WHERE ticketnumber = @ticketNumber";

                using var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ticketNumber", ticketNumber);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                // Return true if the update was successful (at least one row affected)
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine($"Error updating queueentry: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateNoShowStatus(string ticketNumber)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                var query = "UPDATE public.queueentry SET noshow = 1 WHERE ticketNumber=" + ticketNumber;

                using var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@TicketNumber", ticketNumber);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                // Return true if the update was successful (at least one row affected)
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Handle exceptions or log errors here
                Console.WriteLine("Error updating noshow status: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateRecallCount(string ticketNumber, int recallCount)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                var query = "UPDATE public.queueentry " +
                            "SET recallcount = @recallCount " +
                            "WHERE ticketnumber = @ticketNumber";

                using var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@recallCount", recallCount);
                cmd.Parameters.AddWithValue("@ticketNumber", ticketNumber);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                // Return true if the update was successful (at least one row affected)
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine($"Error updating recall count: {ex.Message}");
                return false;
            }
        }

        public async Task<int> GetRecallCountAsync(string ticketNumber)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                var query = "SELECT recallcount FROM public.queueentry WHERE ticketnumber = @TicketNumber";

                using var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@TicketNumber", ticketNumber);

                var result = await cmd.ExecuteScalarAsync();

                // Check if the result is not null and is convertible to int
                if (result != null && int.TryParse(result.ToString(), out int recallCount))
                {
                    return recallCount;
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine($"Error fetching recall count: {ex.Message}");
            }

            return 0;
        }

        public async Task<List<QueueEntry>> GetQueueEntriesByCriteria(string servicePoint)
        {
            var queueEntries = new List<QueueEntry>();

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                var query = "SELECT id, ticketnumber, servicepoint, customername, checkintime, servicepointid " +
                    $"FROM public.queueentry WHERE servicepoint = '{servicePoint}' AND recallcount<3 AND noshow = 0 AND markfinished = 0";



                using var cmd = new NpgsqlCommand(query, connection);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var queueEntry = new QueueEntry
                    {
                        Id = reader.GetInt32(0),
                        TicketNumber = reader.GetString(1),
                        ServicePoint = reader.GetString(2),
                        CustomerName = reader.GetString(3),
                        CheckinTime = reader.GetDateTime(4)
                    };

                    queueEntries.Add(queueEntry);
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine($"Error fetching queue entries: {ex.Message}");
            }

            return queueEntries;
        }


        public async Task<List<(string ServicePoint, int ServiceTypeId)>> GetServiceProviderDetailsAsync()
        {
            var serviceProviderDetails = new List<(string ServicePoint, int ServiceTypeId)>();

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                var query = "SELECT servicepoint, servicetypeid FROM public.serviceproviders";

                using var cmd = new NpgsqlCommand(query, connection);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var servicePoint = reader.GetString(0);
                    var serviceTypeId = reader.GetInt32(1);

                    serviceProviderDetails.Add((servicePoint, serviceTypeId));
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine($"Error fetching service provider details: {ex.Message}");
            }

            return serviceProviderDetails;
        }

        public async Task<bool> TransferTicketToService(string ticketNumber, string servicePoint, int servicePointId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                var query = "UPDATE public.queueentry " +
                            "SET servicepoint = @servicePoint, servicepointid = @servicePointId " +
                            "WHERE ticketnumber = @ticketNumber";

                using var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@servicePoint", servicePoint);
                cmd.Parameters.AddWithValue("@servicePointId", servicePointId);
                cmd.Parameters.AddWithValue("@ticketNumber", ticketNumber);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                // Return true if the update was successful (at least one row affected)
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine($"Error updating queueentry service point: {ex.Message}");
                return false;
            }
        }


        public async Task<AdminViewModel> AdminLoginAsync(string usernameOrEmail, string password)
        {
            AdminViewModel admin = null;

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                var query = "SELECT adminid, username, password, email, phonenumber, createdat " +
                            "FROM public.admins " +
                            "WHERE (Username = @UsernameOrEmail OR Email = @UsernameOrEmail) " +
                            "AND password = @Password";

                using var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("UsernameOrEmail", usernameOrEmail);
                cmd.Parameters.AddWithValue("Password", password);

                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    admin = new AdminViewModel
                    {
                        AdminId = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Password = reader.GetString(2),
                        Email = reader.GetString(3),
                        PhoneNumber = reader.GetString(4),
                        CreatedAt = reader.GetDateTime(5)
                    };
                }
            }
            catch (Exception ex)
            {
                // Handle exception or log the error
                Console.WriteLine($"Error during admin login: {ex.Message}");
            }

            return admin;
        }
        public async Task<List<ServedCustomers>> GetFinishedEntries()
        {
            var finishedEntries = new List<ServedCustomers>();

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                var query = @"SELECT F.finishedid, Q.customername, F.ticketnumber, F.servicepointid, Q.servicepoint, Q.checkintime, F.markedtime 
                              FROM public.finishedtable AS F, public.queueentry AS Q  
                              WHERE F.ticketnumber = Q.ticketnumber";

                using var cmd = new NpgsqlCommand(query, connection);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var finishedEntry = new ServedCustomers
                    {
                        FinishedId = reader.GetInt32(0),
                        CustomerName = reader.GetString(1),
                        TicketNumber = reader.GetString(2),
                        ServicePointId = reader.GetInt32(3),
                        ServicePoint = reader.GetString(4),
                        CheckInTime = reader.GetDateTime(5),
                        MarkedTime = reader.GetDateTime(6)
                    };

                    finishedEntries.Add(finishedEntry);
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine($"Error fetching finished entries: {ex.Message}");
            }

            return finishedEntries;
        }
        public async Task<bool> InsertServiceProviderAsync(ServiceProviderModel serviceProvider)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                var query = @"INSERT INTO public.serviceproviders(
                          username, email, phone, passwordhash, registrationdate, servicepoint, servicetypeid)
                      VALUES (@username, @email, @phone,
                              @passwordHash, @registrationDate, @servicePoint, @serviceTypeId)";

                using var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@username", serviceProvider.Username);
                cmd.Parameters.AddWithValue("@email", serviceProvider.Email);
                cmd.Parameters.AddWithValue("@phone", serviceProvider.Phone);
                cmd.Parameters.AddWithValue("@passwordHash", serviceProvider.PasswordHash);
                cmd.Parameters.AddWithValue("@registrationDate", serviceProvider.RegistrationDate);
                cmd.Parameters.AddWithValue("@servicePoint", serviceProvider.ServicePoint);
                cmd.Parameters.AddWithValue("@serviceTypeId", serviceProvider.ServiceTypeId);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                // Return true if the insertion was successful (at least one row affected)
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine($"Error inserting into serviceproviders: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> InsertServiceTypeAsync(ServiceTypeModel serviceType)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                var query = @"INSERT INTO public.servicetypes(
                           name, description)
                      VALUES ( @name, @description)";

                using var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@name", serviceType.Name);
                cmd.Parameters.AddWithValue("@description", serviceType.Description ?? (object)DBNull.Value);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                // Return true if the insertion was successful (at least one row affected)
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine($"Error inserting into servicetypes: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> IsUserAuthorizedAsync(LoginViewModel admin)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();
                var query = "SELECT adminid, username, password, email, phonenumber, createdat " +
                            "FROM public.admins " +
                            "WHERE (Username = @UsernameOrEmail OR Email = @UsernameOrEmail) " +
                            "AND password = @Password";
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", admin.UsernameOrEmail);
                command.Parameters.AddWithValue("@password", admin.Password);

                var result = await command.ExecuteScalarAsync();
                return result != null && Convert.ToInt32(result) > 0;
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine($"Error checking user authorization: {ex.Message}");
                return false;
            }
        }

        public async Task<List<QueueEntry>> GetQueueEntriesWithNoShowAsync()
        {
            var queueEntriesWithNoShow = new List<QueueEntry>();

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                var query = "SELECT id, ticketnumber, servicepoint, customername, checkintime " +
                            "FROM public.queueentry WHERE noshow = 1";

                using var cmd = new NpgsqlCommand(query, connection);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var queueEntry = new QueueEntry
                    {
                        Id = reader.GetInt32(0),
                        TicketNumber = reader.GetString(1),
                        ServicePoint = reader.GetString(2),
                        CustomerName = reader.GetString(3),
                        CheckinTime = reader.GetDateTime(4),
                    };

                    queueEntriesWithNoShow.Add(queueEntry);
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine($"Error fetching queue entries with no show: {ex.Message}");
            }

            return queueEntriesWithNoShow;
        }

        public List<ServiceProviderModel> GetServiceProviders()
        {
            var serviceProviders = new List<ServiceProviderModel>();

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                connection.Open();

                var query = "SELECT id, username, email, phone, passwordhash, registrationdate, servicepoint, servicetypeid, isauthorized FROM public.serviceproviders";

                using var cmd = new NpgsqlCommand(query, connection);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var serviceProvider = new ServiceProviderModel
                    {
                        Id = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Email = reader.GetString(2),
                        Phone = reader.GetString(3),
                        PasswordHash = reader.GetString(4),
                        RegistrationDate = reader.GetDateTime(5),
                        ServicePoint = reader.GetString(6),
                        ServiceTypeId = reader.GetInt32(7),
                        IsAuthorized = reader.GetBoolean(8)
                    };

                    serviceProviders.Add(serviceProvider);
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine($"Error fetching service providers: {ex.Message}");
            }

            return serviceProviders;
        }


        public async Task<int> GetCustomersServedCountAsync(DateTime date)
        {
            int customersServedCount = 0;

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                var query = "SELECT COUNT(*) FROM public.finishedtable WHERE DATE(markedtime) = @date";

                using var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("date", date.Date);

                object result = await cmd.ExecuteScalarAsync();

                if (result != null && result != DBNull.Value)
                {
                    customersServedCount = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                // Handle exception or log the error
                Console.WriteLine($"Error fetching customers served count: {ex.Message}");
            }

            return customersServedCount;
        }

        public async Task<List<AverageServiceTimePerServicePoint>> GetAverageServiceTimePerServicePointAsync()
        {
            var result = new List<AverageServiceTimePerServicePoint>();

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                var query = @"
            SELECT 
                Q.servicepointid,
                AVG(EXTRACT(EPOCH FROM (F.markedtime - Q.checkintime))) AS average_seconds
            FROM 
                public.finishedtable AS F
            INNER JOIN 
                public.queueentry AS Q ON F.ticketnumber = Q.ticketnumber
            GROUP BY 
                Q.servicepointid;
        ";

                using var cmd = new NpgsqlCommand(query, connection);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    int servicePointId = reader.GetInt32(0);
                    double averageSeconds = reader.GetDouble(1); // Retrieve the average seconds

                    var averageServiceTimePerServicePoint = new AverageServiceTimePerServicePoint
                    {
                        ServicePointId = servicePointId,
                        AverageServiceTime = TimeSpan.FromSeconds(averageSeconds) // Convert seconds to TimeSpan
                    };

                    result.Add(averageServiceTimePerServicePoint);
                }
            }
            catch (Exception ex)
            {
                // Handle exception or log the error
                Console.WriteLine($"Error fetching average service time: {ex.Message}");
            }

            return result;
        }

        public async Task<string> GetServiceTypeNameByIdAsync(int id)
        {
            string serviceName = null;

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                var query = "SELECT name FROM public.servicetypes WHERE id = @Id";

                using var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", id);

                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    serviceName = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine($"Error fetching service type name by ID: {ex.Message}");
            }

            return serviceName;
        }


        public async Task<bool> UpdateServiceProviderAuthorization(int serviceProviderId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                // Construct the SQL command with a parameterized query to avoid SQL injection
                var sql = "UPDATE public.serviceproviders SET isauthorized = true WHERE id = @ServiceProviderId";

                await using var cmd = new NpgsqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@ServiceProviderId", serviceProviderId);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                return rowsAffected > 0; // Return true if any rows were affected by the query
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                Console.WriteLine($"Error updating service provider authorization: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> DeleteServiceProviderById(int serviceProviderId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                var deleteCommandText = $"DELETE FROM public.serviceproviders WHERE id = {serviceProviderId}";


                await using var cmd = new NpgsqlCommand(deleteCommandText, connection);
                cmd.Parameters.AddWithValue("@ServiceProviderId", serviceProviderId);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                return rowsAffected > 0; // Return true if any rows were affected by the query
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                Console.WriteLine($"Error updating service provider authorization: {ex.Message}");
                return false;
            }
        }

       

        public async Task<List<ServiceStatistics>> GetServiceStatistics()
        {
            List<ServiceStatistics> statistics = new List<ServiceStatistics>();

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                var query = @"
            WITH FinishedCount AS (
                SELECT COUNT(*) AS finished_count
                FROM public.finishedtable
                WHERE DATE(markedtime) = CURRENT_DATE
            ),
            AverageTimes AS (
                SELECT 
                    Q.servicepointid,
                    AVG(EXTRACT(EPOCH FROM (F.markedtime - Q.checkintime))) AS average_seconds
                FROM 
                    public.finishedtable AS F
                INNER JOIN 
                    public.queueentry AS Q ON F.ticketnumber = Q.ticketnumber
                GROUP BY 
                    Q.servicepointid
            ),
            NoShowEntries AS (
                SELECT 
                    id, ticketnumber, servicepoint, customername, servicepointid
                FROM 
                    public.queueentry
                WHERE 
                    noshow = 1
            )
            SELECT 
                FC.finished_count,
                AT.servicepointid AS ServicePointId,
                AT.average_seconds AS AverageSeconds,
                NE.id AS Id,
                NE.ticketnumber AS TicketNumber,
                NE.servicepoint AS ServicePoint,
                NE.customername AS CustomerName,
                NE.servicepointid AS NoShowServicePointId
            FROM 
                FinishedCount FC
            LEFT JOIN 
                AverageTimes AT ON 1=1
            LEFT JOIN 
                NoShowEntries NE ON NE.servicepointid = AT.servicepointid";

                using var cmd = new NpgsqlCommand(query, connection);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var stat = new ServiceStatistics
                    {
                        FinishedCount = Convert.ToInt32(reader["finished_count"]),
                        ServicePointId = Convert.ToInt32(reader["ServicePointId"]),
                        AverageSeconds = Convert.ToDouble(reader["AverageSeconds"]),
                        Id = Convert.ToInt32(reader["Id"]),
                        TicketNumber = reader["TicketNumber"].ToString(),
                        ServicePoint = reader["ServicePoint"].ToString(),
                        CustomerName = reader["CustomerName"].ToString(),
                        NoShowServicePointId = Convert.ToInt32(reader["NoShowServicePointId"])
                    };

                    statistics.Add(stat);
                }
            }
            catch (Exception ex)
            {
                // Handle exception or log the error
                Console.WriteLine($"Error getting service statistics: {ex.Message}");
            }

            return statistics;
        }


    }
}
