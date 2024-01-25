using Queue_Management_System.Models;
using Npgsql;
using System.Drawing.Printing;

namespace Queue_Management_System.Repositories
{
    public interface ITicketRepository
    {
        Task AddTicket(TicketModel ticket);

        Task<TicketModel> GetUnservedTicketByTicketNumber(string ticketNumber);

        Task MarkTicketAsShowedUp(string ticketNumber, DateTime showUpTime, string servicePointId);

        Task SetTicketFinishTime(string ticketNumber, DateTime finishTime);

        Task<IEnumerable<ServicePointAnalyticModel>> GetServicePointAnalytics();

    
    }

    /*public class MockTicketRepository : ITicketRepository
    {
        private static readonly List<TicketModel> Tickets = new List<TicketModel>();

        public void AddTicket(TicketModel ticket)
        {
            Tickets.Add(ticket);
        }

        public TicketModel GetUnservedTicketByTicketNumber(string ticketNumber)
        {
            return Tickets.Find(ticket => (ticket.TicketNumber == ticketNumber) && (ticket.ShowedUp == null));
        }

        public void MarkTicketAsShowedUp(string ticketNumber, DateTime showUpTime)
        {
            var ticket = Tickets.Find(ticket => (ticket.TicketNumber == ticketNumber) && (ticket.ShowedUp == false));
            if (ticket!=null)
            {
                ticket.ShowedUp = true;
                ticket.TimeShowedUp = showUpTime;
            }
        }

        public void SetTicketFinishTime(string ticketNumber, DateTime finishTime)
        {
            var ticket = Tickets.Find(ticket => (ticket.TicketNumber == ticketNumber) && (ticket.TimeFinished == null));
            if (ticket!=null)
            {
                ticket.ShowedUp = true;
                ticket.TimeFinished = finishTime;
            }
        }

    }*/

    public class TicketRepository : ITicketRepository
    {
        private readonly IConfiguration _configuration;
        
        private readonly NpgsqlDataSource dataSource;

        public TicketRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            string connectionString = _configuration.GetConnectionString("qmsdb");
            dataSource = NpgsqlDataSource.Create(connectionString);
        }

        public async Task AddTicket(TicketModel ticket)
        {
            string ticketNumber = ticket.TicketNumber;
            string serviceId = ticket.ServiceId;
            string? servicePointId = ticket.ServicePointId;
            DateTime timePrinted = ticket.TimePrinted;
            bool? showedUp = ticket.ShowedUp;
            DateTime? timeShowedUp = ticket.TimeShowedUp;
            DateTime? timeFinished = ticket.TimeFinished;
            string? serviceProviderId = ticket.ServiceProviderId;


            await using var connection = await dataSource.OpenConnectionAsync();
            //string querystring = $"INSERT INTO services (id, description) VALUES ('{id}', '{description}')";
            await using var command = new NpgsqlCommand("INSERT INTO tickets (ticket_number, service_id, service_point_id, time_printed, showed_up, time_showed_up, time_finished, service_provider_id) VALUES ($1, $2, $3, $4, $5, $6, $7, $8)", connection)
            {
                Parameters = 
                {
                    new() {Value = ticketNumber},
                    new() {Value = serviceId},
                    new() {Value = servicePointId ?? (object)DBNull.Value},
                    new() {Value = timePrinted},
                    new() {Value = showedUp ?? (object)DBNull.Value},
                    new() {Value = timeShowedUp ?? (object)DBNull.Value},
                    new() {Value = timeFinished ?? (object)DBNull.Value},
                    new() {Value = serviceProviderId ?? (object)DBNull.Value},
                }
            };
            await command.ExecuteNonQueryAsync();

        }
    
        public async Task<TicketModel> GetUnservedTicketByTicketNumber(string ticketNumber)
        {
            
            await using var connection = await dataSource.OpenConnectionAsync();
            //string querystring = $"SELECT * FROM services WHERE id='{id}'";
            await using var command = new NpgsqlCommand("SELECT * FROM services WHERE ticket_number=$1 AND showed_up=$2", connection)
            {
                Parameters = 
                {
                    new() {Value = ticketNumber},
                    new() {Value = false}
                }
            };
            await using var reader = await command.ExecuteReaderAsync();

            TicketModel? ticket = null;
            while (await reader.ReadAsync())
            {
                ticket = new TicketModel
                {
                    TicketNumber = reader.GetString(0),
                    ServiceId = reader.GetString(1),
                    ServicePointId = reader.IsDBNull(2) ? null : reader.GetString(2),
                    TimePrinted = reader.GetDateTime(3),
                    ShowedUp = reader.IsDBNull(4) ? null : reader.GetBoolean(4),
                    TimeShowedUp = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                    TimeFinished = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                    ServiceProviderId = reader.IsDBNull(7) ? null : reader.GetString(7),
                };
            }

            return ticket;

        }
    
        public async Task MarkTicketAsShowedUp(string ticketNumber, DateTime showUpTime, string servicePointId)
        {
            Console.WriteLine("mark"+ticketNumber);
            await using var connection = await dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("UPDATE tickets SET (service_point_id, showed_up, time_showed_up) = ($1, $2, $3) WHERE (ticket_number = $4 AND showed_up IS NULL)", connection)
            {
                Parameters = 
                {
                    new() {Value = servicePointId},
                    new() {Value = true},
                    new() {Value = showUpTime},
                    new() {Value = ticketNumber},
                    //new() {Value = DBNull.Value}
                }
            };
            await command.ExecuteNonQueryAsync();
            Console.WriteLine("successfull");

        }

        public async Task SetTicketFinishTime(string ticketNumber, DateTime finishTime)
        {

            await using var connection = await dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("UPDATE tickets SET time_finished = $1 WHERE ticket_number = $2 AND time_finished IS NULL", connection)
            {
                Parameters = 
                {
                    new() {Value = finishTime},
                    new() {Value = ticketNumber},
                    //new() {Value = DBNull.Value},
                }
            };

            await command.ExecuteNonQueryAsync();
            Console.WriteLine("settofinish");

        }

        public async Task<IEnumerable<ServicePointAnalyticModel>> GetServicePointAnalytics()
        {
            await using var connection = await dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("SELECT service_point_id, AVG(time_showed_up - time_printed), AVG(time_finished - time_showed_up), COUNT(service_point_id) FROM tickets WHERE showed_up=$1 GROUP BY service_point_id", connection)
            {
                Parameters = 
                {
                    new() {Value = true}
                }
            };

            await using var reader = await command.ExecuteReaderAsync();
            var result = new List<ServicePointAnalyticModel>();
            while (await reader.ReadAsync())
            {
                var analytic = new ServicePointAnalyticModel
                {
                    ServicePointId = reader.GetString(0),
                    AverageWaitingTime = reader.GetTimeSpan(1),
                    AverageServiceTime = reader.GetTimeSpan(2),
                    TotalCustomers = reader.GetInt32(3)
                };
                result.Add(analytic);
            }
            foreach (var analytic in result)
            {
                Console.WriteLine(analytic.AverageServiceTime);
            }
            return result;
        }
    }
}

