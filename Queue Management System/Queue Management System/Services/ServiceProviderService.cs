using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using Queue_Management_System.Models;

public class ServiceProviderService : IServiceProviderService
{
    private readonly string _connectionString;
    private readonly ILogger<ServicePointService> _logger;

    public ServiceProviderService(IConfiguration configuration, ILogger<ServicePointService> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _logger = logger;
    }

    public List<ServiceProvider> GetServiceProviders()
    {
        var serviceProviders = new List<ServiceProvider>();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (var command = new NpgsqlCommand("SELECT ServiceProviderId, Username, Role, PasswordHash FROM ServiceProvider", connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var serviceProvider = new ServiceProvider
                    {
                        ServiceProviderId = reader.GetInt32(reader.GetOrdinal("ServiceProviderId")),
                        Username = reader.GetString(reader.GetOrdinal("Username")),
                        Password = reader.GetString(reader.GetOrdinal("PasswordHash")),
                        Role = reader.GetString(reader.GetOrdinal("Role"))
                    };

                    serviceProviders.Add(serviceProvider);
                }
            }
        }

        return serviceProviders;
    }

    public ServiceProvider GetServiceProviderById(int serviceProviderId)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        using (var command = new NpgsqlCommand("SELECT * FROM ServiceProvider WHERE ServiceProviderId = @ServiceProviderId", connection))
        {
            connection.Open();
            command.Parameters.AddWithValue("@ServiceProviderId", serviceProviderId);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    int id = reader.GetInt32(reader.GetOrdinal("ServiceProviderId"));
                    string name = reader.GetString(reader.GetOrdinal("Username"));
                    string password = reader.GetString(reader.GetOrdinal("PasswordHash"));
                    string role = reader.GetString(reader.GetOrdinal("Role"));

                    return new ServiceProvider { ServiceProviderId = id, Username = name, Password = password, Role = role };
                }
            }
        }

        return null;
    }

    public void AddServiceProviderWithServicePoints(ServiceProvider serviceProvider, List<int> servicePointIds)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    using (var insertServiceProviderCommand = new NpgsqlCommand("INSERT INTO ServiceProvider (Username, PasswordHash, Role) VALUES (@Username, @PasswordHash, @Role) RETURNING ServiceProviderId", connection))
                    {
                        insertServiceProviderCommand.Parameters.AddWithValue("@Username", serviceProvider.Username);
                        insertServiceProviderCommand.Parameters.AddWithValue("@PasswordHash", serviceProvider.Password);
                        insertServiceProviderCommand.Parameters.AddWithValue("@Role", serviceProvider.Role);

                        serviceProvider.ServiceProviderId = (int)insertServiceProviderCommand.ExecuteScalar();
                    }

                    foreach (int servicePointId in servicePointIds)
                    {
                        using (var insertServicePointCommand = new NpgsqlCommand("INSERT INTO ServiceProviderServicePoint (ServiceProviderId, ServicePointId) VALUES (@ServiceProviderId, @ServicePointId)", connection))
                        {
                            insertServicePointCommand.Parameters.AddWithValue("@ServiceProviderId", serviceProvider.ServiceProviderId);
                            insertServicePointCommand.Parameters.AddWithValue("@ServicePointId", servicePointId);

                            insertServicePointCommand.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }

    public List<ServiceProvider> GetServiceProvidersWithServicePoints()
    {
        var serviceProviders = new List<ServiceProvider>();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (var command = new NpgsqlCommand("SELECT sp.ServiceProviderId, sp.Username, sp.Role, sp.PasswordHash, spsp.ServicePointId, spoint.ServicePointName " +
                "FROM ServiceProvider sp " +
                "LEFT JOIN ServiceProviderServicePoint spsp ON sp.ServiceProviderId = spsp.ServiceProviderId " +
                "LEFT JOIN ServicePoint spoint ON spsp.ServicePointId = spoint.ServicePointId", connection))
            using (var reader = command.ExecuteReader())
            {
                int currentServiceProviderId = 0;
                ServiceProvider currentServiceProvider = null;

                while (reader.Read())
                {
                    int serviceProviderId = reader.GetInt32(reader.GetOrdinal("ServiceProviderId"));

                    if (serviceProviderId != currentServiceProviderId)
                    {
                        currentServiceProvider = new ServiceProvider
                        {
                            ServiceProviderId = serviceProviderId,
                            Username = reader.GetString(reader.GetOrdinal("Username")),
                            Password = reader.GetString(reader.GetOrdinal("PasswordHash")),
                            Role = reader.GetString(reader.GetOrdinal("Role")),
                            ServicePoints = new List<ServicePoint>()
                        };

                        serviceProviders.Add(currentServiceProvider);
                        currentServiceProviderId = serviceProviderId;
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("ServicePointId")))
                    {
                        var servicePoint = new ServicePoint
                        {
                            ServicePointId = reader.GetInt32(reader.GetOrdinal("ServicePointId")),
                            ServicePointName = reader.GetString(reader.GetOrdinal("ServicePointName"))
                        };

                        currentServiceProvider.ServicePoints.Add(servicePoint);
                    }
                }
            }
        }

        return serviceProviders;
    }

    public ServiceProvider GetServiceProviderWithServicePointsById(int serviceProviderId)
    {
        ServiceProvider serviceProvider = null;

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (var command = new NpgsqlCommand("SELECT sp.ServiceProviderId, sp.Username, sp.Role, sp.PasswordHash, spsp.ServicePointId, spoint.ServicePointName " +
                "FROM ServiceProvider sp " +
                "LEFT JOIN ServiceProviderServicePoint spsp ON sp.ServiceProviderId = spsp.ServiceProviderId " +
                "LEFT JOIN ServicePoint spoint ON spsp.ServicePointId = spoint.ServicePointId " +
                "WHERE sp.ServiceProviderId = @ServiceProviderId", connection))
            {
                command.Parameters.AddWithValue("@ServiceProviderId", serviceProviderId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (serviceProvider == null)
                        {
                            serviceProvider = new ServiceProvider
                            {
                                ServiceProviderId = reader.GetInt32(reader.GetOrdinal("ServiceProviderId")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                Password = reader.GetString(reader.GetOrdinal("PasswordHash")),
                                Role = reader.GetString(reader.GetOrdinal("Role")),
                                ServicePoints = new List<ServicePoint>()
                            };
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("ServicePointId")))
                        {
                            var servicePoint = new ServicePoint
                            {
                                ServicePointId = reader.GetInt32(reader.GetOrdinal("ServicePointId")),
                                ServicePointName = reader.GetString(reader.GetOrdinal("ServicePointName"))
                            };

                            serviceProvider.ServicePoints.Add(servicePoint);
                        }
                    }
                }
            }
        }

        return serviceProvider;
    }

    public void UpdateServiceProviderWithServicePoints(ServiceProvider serviceProvider, List<int> newServicePointIds)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    using (var updateServiceProviderCommand = new NpgsqlCommand("UPDATE ServiceProvider SET Username = @Username, PasswordHash = @PasswordHash, Role = @Role WHERE ServiceProviderId = @ServiceProviderId", connection))
                    {
                        updateServiceProviderCommand.Parameters.AddWithValue("@Username", serviceProvider.Username);
                        updateServiceProviderCommand.Parameters.AddWithValue("@PasswordHash", serviceProvider.Password);
                        updateServiceProviderCommand.Parameters.AddWithValue("@Role", serviceProvider.Role);
                        updateServiceProviderCommand.Parameters.AddWithValue("@ServiceProviderId", serviceProvider.ServiceProviderId);

                        updateServiceProviderCommand.ExecuteNonQuery();
                    }

                    using (var deleteServicePointsCommand = new NpgsqlCommand("DELETE FROM ServiceProviderServicePoint WHERE ServiceProviderId = @ServiceProviderId", connection))
                    {
                        deleteServicePointsCommand.Parameters.AddWithValue("@ServiceProviderId", serviceProvider.ServiceProviderId);

                        deleteServicePointsCommand.ExecuteNonQuery();
                    }

                    foreach (int servicePointId in newServicePointIds)
                    {
                        using (var insertServicePointCommand = new NpgsqlCommand("INSERT INTO ServiceProviderServicePoint (ServiceProviderId, ServicePointId) VALUES (@ServiceProviderId, @ServicePointId)", connection))
                        {
                            insertServicePointCommand.Parameters.AddWithValue("@ServiceProviderId", serviceProvider.ServiceProviderId);
                            insertServicePointCommand.Parameters.AddWithValue("@ServicePointId", servicePointId);

                            insertServicePointCommand.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }

    public void DeleteServiceProviderWithServicePoints(int serviceProviderId)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    using (var deleteServicePointsCommand = new NpgsqlCommand("DELETE FROM ServiceProviderServicePoint WHERE ServiceProviderId = @ServiceProviderId", connection))
                    {
                        deleteServicePointsCommand.Parameters.AddWithValue("@ServiceProviderId", serviceProviderId);

                        deleteServicePointsCommand.ExecuteNonQuery();
                    }

                    using (var deleteServiceProviderCommand = new NpgsqlCommand("DELETE FROM ServiceProvider WHERE ServiceProviderId = @ServiceProviderId", connection))
                    {
                        deleteServiceProviderCommand.Parameters.AddWithValue("@ServiceProviderId", serviceProviderId);

                        deleteServiceProviderCommand.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }

    public bool IsUsernameUnique(string username)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (var command = new NpgsqlCommand("SELECT COUNT(*) = 0 AS IsUsernameUnique FROM ServiceProvider WHERE LOWER(Username) = LOWER(@Username)", connection))
            {
                command.Parameters.AddWithValue("@Username", username);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetBoolean(0);
                    }
                }
            }
        }

        return false;
    }

}
