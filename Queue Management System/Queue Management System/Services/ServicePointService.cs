
using System.Net;
using FastReport.AdvMatrix;
using Npgsql;
using Queue_Management_System.Models;

public class ServicePointService : IServicePointService {


     private readonly string _connectionString;

    private readonly ILogger<ServicePointService> _logger;

    public ServicePointService(IConfiguration configuration, ILogger<ServicePointService> logger)
    {

        _connectionString = configuration.GetConnectionString("DefaultConnection");

        _logger = logger;
    }


    public List <Ticket> findTicketsPerServicePoint (int ServicePointId) {

        List<Ticket> tickets = new List<Ticket>();

    using (var connection = new NpgsqlConnection(_connectionString))
    {
        connection.Open();

        using (var command = new NpgsqlCommand())
        {
            command.Connection = connection;

            command.CommandText = "SELECT TicketId, IssueTime, Status, ServicePointId FROM Ticket WHERE ServicePointId = @ServicePointId";
            command.Parameters.AddWithValue("@ServicePointId", ServicePointId);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Ticket ticket = new Ticket
                    {
                        TicketId = (int)reader["TicketId"],
                        IssueTime = (DateTime)reader["IssueTime"],
                        Status = reader.GetString(reader.GetOrdinal("Status")),
                        ServicePointId = reader.GetInt32(reader.GetOrdinal("ServicePointId"))
                    };

                    tickets.Add(ticket);
                }
            }
        }
    }

    return tickets;


    }

    public List<Queue_Management_System.Models.ServicePoint> GetServicePoints()
{
    var servicePoints = new List<Queue_Management_System.Models.ServicePoint>();

    using (var connection = new NpgsqlConnection(_connectionString))
    {
        connection.Open();

        using (var command = new NpgsqlCommand("SELECT ServicePointId, ServicePointName, Description FROM ServicePoint", connection))
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var servicePoint = new Queue_Management_System.Models.ServicePoint
                    {
                        ServicePointId = reader.GetInt32(0),
                        ServicePointName = reader.GetString(1),
                        Description = reader.GetString(2)
                    };

                    servicePoints.Add(servicePoint);
                }
            }
        }
    }

    return servicePoints;
}

public Queue_Management_System.Models.ServicePoint GetServicePointById(int servicePointId)
{

    
    using (var connection = new NpgsqlConnection(_connectionString))
    {
        connection.Open();

        using (var command = new NpgsqlCommand("SELECT * FROM ServicePoint WHERE ServicePointId = @ServicePointId", connection))
        {
            command.Parameters.AddWithValue("ServicePointId", servicePointId);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    int id = reader.GetInt32(reader.GetOrdinal("ServicePointId"));
                    string name = reader.GetString(reader.GetOrdinal("ServicePointName"));
                    string description = reader.GetString(reader.GetOrdinal("Description"));

                    return new Queue_Management_System.Models.ServicePoint { ServicePointId = id, ServicePointName = name, Description = description };
                }
            }
        }
    }

    return null;
}

public Ticket GetCurrentTicketPerServicePoint (int servicePointId) {
     Ticket currentTicket = null;

    using (var connection = new NpgsqlConnection(_connectionString)) {
      
      connection.Open();
    
    using (var command = new NpgsqlCommand()) 
    {
      command.Connection = connection;

      command.CommandText = "SELECT TicketId, IssueTime, ServicePointId FROM Ticket WHERE ServicePointId = @servicePointId ORDER BY IssueTime ASC LIMIT 1";

       command.Parameters.AddWithValue("@servicePointId", servicePointId);

      using (var reader = command.ExecuteReader()) {

        if (reader.Read()) {
            currentTicket = new Ticket {
                TicketId = reader.GetInt32(reader.GetOrdinal("TicketId")),
                IssueTime = reader.GetDateTime(reader.GetOrdinal("IssueTime")),
            };
        }
      }

    }
   
}

   return currentTicket;

}


    public Ticket GetNextTicketPerServicePoint(int servicePointId)
    {
        // Get the current ticket
        Ticket currentTicket = GetCurrentTicketPerServicePoint(servicePointId);

        if (currentTicket != null)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT TicketId, IssueTime, ServicePointId " +
                                          "FROM Ticket " +
                                          "WHERE ServicePointId = @servicePointId AND IssueTime > @currentTicketIssueTime " +
                                          "ORDER BY IssueTime ASC " +
                                          "LIMIT 1;";

                    command.Parameters.AddWithValue("servicePointId", servicePointId);
                    command.Parameters.AddWithValue("currentTicketIssueTime", currentTicket.IssueTime);

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
    
                            return new Ticket
                            {
                                TicketId = reader.GetInt32(0),
                                IssueTime = reader.GetDateTime(1),
                                //ServicePointId = reader.GetInt32(2)
                            };
                        }
                    }
                }
            }
        }
        return null;
    }


}