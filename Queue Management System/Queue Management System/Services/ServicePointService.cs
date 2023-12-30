using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using Queue_Management_System.Models;

public class ServicePointService : IServicePointService
{
    private readonly string _connectionString;
    private readonly ILogger<ServicePointService> _logger;

    public ServicePointService(IConfiguration configuration, ILogger<ServicePointService> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _logger = logger;
    }

    public List<Ticket> findTicketsPerServicePoint(int servicePointId)
    {
        List<Ticket> tickets = new List<Ticket>();

        using (var connection = new NpgsqlConnection(_connectionString))
        using (var command = new NpgsqlCommand())
        {
            connection.Open();
            command.Connection = connection;
            command.CommandText = "SELECT TicketId, IssueTime, Status, ServicePointId FROM Ticket WHERE ServicePointId = @ServicePointId";
            command.Parameters.AddWithValue("@ServicePointId", servicePointId);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Ticket ticket = CreateTicketFromReader(reader);
                    tickets.Add(ticket);
                }
            }
        }

        return tickets;
    }

    private Ticket CreateTicketFromReader(NpgsqlDataReader reader)
    {
        return new Ticket
        {
            TicketId = (int)reader["TicketId"],
            IssueTime = (DateTime)reader["IssueTime"],
            Status = reader.GetString(reader.GetOrdinal("Status")),
            ServicePointId = reader.GetInt32(reader.GetOrdinal("ServicePointId"))
        };
    }

    public List<ServicePoint> GetServicePoints()
    {
        var servicePoints = new List<ServicePoint>();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
        connection.Open();    
        using (var command = new NpgsqlCommand("SELECT ServicePointId, ServicePointName, Description FROM ServicePoint", connection))
        using (var reader = command.ExecuteReader())
        {
            //connection.Open();
            while (reader.Read())
            {
                var servicePoint = new ServicePoint
                {
                    ServicePointId = reader.GetInt32(0),
                    ServicePointName = reader.GetString(1),
                    Description = reader.GetString(2)
                };

                servicePoints.Add(servicePoint);
            }
        }

        }

        return servicePoints;
    }

    public ServicePoint GetServicePointById(int servicePointId)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        using (var command = new NpgsqlCommand("SELECT * FROM ServicePoint WHERE ServicePointId = @ServicePointId", connection))
        {
            connection.Open();
            command.Parameters.AddWithValue("@ServicePointId", servicePointId);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    int id = reader.GetInt32(reader.GetOrdinal("ServicePointId"));
                    string name = reader.GetString(reader.GetOrdinal("ServicePointName"));
                    string description = reader.GetString(reader.GetOrdinal("Description"));

                    return new ServicePoint { ServicePointId = id, ServicePointName = name, Description = description };
                }
            }
        }

        return null;
    }

    public Ticket GetCurrentTicketPerServicePoint(int servicePointId)
    {
        Ticket currentTicket = null;

        using (var connection = new NpgsqlConnection(_connectionString))
        using (var command = new NpgsqlCommand())
        {
            connection.Open();
            command.Connection = connection;
            command.CommandText = "SELECT TicketId, IssueTime, ServicePointId FROM Ticket WHERE ServicePointId = @servicePointId ORDER BY IssueTime ASC LIMIT 1";
            command.Parameters.AddWithValue("@servicePointId", servicePointId);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    currentTicket = new Ticket
                    {
                        TicketId = reader.GetInt32(reader.GetOrdinal("TicketId")),
                        IssueTime = reader.GetDateTime(reader.GetOrdinal("IssueTime")),
                    };
                }
            }
        }

        return currentTicket;
    }

    public Ticket GetNextTicketPerServicePoint(int servicePointId)
    {
        Ticket currentTicket = GetCurrentTicketPerServicePoint(servicePointId);

        if (currentTicket != null)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            using (var command = new NpgsqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT TicketId, IssueTime, ServicePointId " +
                                      "FROM Ticket " +
                                      "WHERE ServicePointId = @servicePointId AND IssueTime > @currentTicketIssueTime " +
                                      "ORDER BY IssueTime ASC " +
                                      "LIMIT 1;";

                command.Parameters.AddWithValue("servicePointId", servicePointId);
                command.Parameters.AddWithValue("currentTicketIssueTime", currentTicket.IssueTime);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Ticket
                        {
                            TicketId = reader.GetInt32(0),
                            IssueTime = reader.GetDateTime(1),
                        };
                    }
                }
            }
        }

        return null;
    }

    public void CreateServicePoint(ServicePoint model)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        using (var command = new NpgsqlCommand())
        {
            connection.Open();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = "INSERT INTO ServicePoint (ServicePointName, Description) VALUES (@ServicePointName, @Description)";
            command.Parameters.AddWithValue("ServicePointName", model.ServicePointName);
            command.Parameters.AddWithValue("Description", model.Description);
            command.ExecuteNonQuery();
        }

        _logger.LogInformation($"ServicePoint '{model.ServicePointName}' created successfully.");
    }

    public void UpdateServicePoint(ServicePoint model)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        using (var command = new NpgsqlCommand("UPDATE ServicePoint SET ServicePointName = @ServicePointName, Description = @Description WHERE ServicePointId = @ServicePointId", connection))
        {
            connection.Open();
            command.Parameters.AddWithValue("@ServicePointName", model.ServicePointName);
            command.Parameters.AddWithValue("@Description", (object)model.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@ServicePointId", model.ServicePointId);
            command.ExecuteNonQuery();
        }
    }

    public void DeleteServicePoint(int id)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        using (var command = new NpgsqlCommand("DELETE FROM ServicePoint WHERE ServicePointId = @ServicePointId", connection))
        {
            connection.Open();
            command.Parameters.AddWithValue("@ServicePointId", id);
            command.ExecuteNonQuery();
        }
    }
}
