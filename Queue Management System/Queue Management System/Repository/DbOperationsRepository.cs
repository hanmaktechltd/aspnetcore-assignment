using Npgsql;
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
                            "ORDER BY id DESC " +
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

                var query = "SELECT Id, Username, Email, Phone, PasswordHash, RegistrationDate, ServicePoint, ServiceTypeId " +
                            "FROM serviceProviders " +
                            "WHERE (Username = @UsernameOrEmail OR Email = @UsernameOrEmail) " +
                            "AND PasswordHash = @Password";

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
                        ServiceTypeId = reader.GetInt32(7)
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

        public async Task<QueueEntry> GetLatestQueueEntryAsync(int servicepointid)
        {
            QueueEntry latestEntry = null;

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                var query = "SELECT id, ticketnumber, servicepoint, customername, checkintime" +
                    " FROM public.queueentry WHERE servicepointid="+ servicepointid + " AND recallcount <3 AND noshow = 0 " +
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

                var query = "UPDATE public.queueentry SET noshow = 1 WHERE ticketNumber="+ ticketNumber ;

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

            return 0; // Default value if recall count retrieval fails
        }

        public async Task<List<QueueEntry>> GetQueueEntriesByCriteria(int servicePointId)
        {
            var queueEntries = new List<QueueEntry>();

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                var query = "SELECT id, ticketnumber, servicepoint, customername, checkintime " +
                            "FROM public.queueentry " +
                            $"WHERE servicepointid = {servicePointId} AND recallcount < 3 AND noshow = 0 AND markfinished = 0 " +
                            "ORDER BY id ASC";

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


    }
}
