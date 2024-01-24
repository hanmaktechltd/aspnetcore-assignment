using Queue_Management_System.Models;
using Npgsql;

namespace Queue_Management_System.Repositories
{
    public interface ITicketRepository
    {
        Task AddTicket(TicketModel ticket);

        Task<TicketModel> GetUnservedTicketByTicketNumber(string ticketNumber);

        //IEnumerable<TicketModel> GetUnservedTicketsByServiceID(string serviceId);

        //void UpdateTicket(TicketModel ticket);

        Task MarkTicketAsShowedUp(string ticketNumber, DateTime showUpTime);

        Task SetTicketFinishTime(string ticketNumber, DateTime finishTime);
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
    
        public async Task MarkTicketAsShowedUp(string ticketNumber, DateTime showUpTime)
        {

            await using var connection = await dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("UPDATE tickets SET (showed_up, time_showed_up) = ($1, $2) WHERE id = $3", connection)
            {
                Parameters = 
                {
                    new() {Value = true},
                    new() {Value = showUpTime},
                    new() {Value = ticketNumber},
                }
            };
            await command.ExecuteNonQueryAsync();

        }

        public async Task SetTicketFinishTime(string ticketNumber, DateTime finishTime)
        {

            await using var connection = await dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("UPDATE tickets SET (time_finsihed) = ($1) WHERE id = $2", connection)
            {
                Parameters = 
                {
                    new() {Value = finishTime},
                    new() {Value = ticketNumber},
                }
            };

            await command.ExecuteNonQueryAsync();

        }
    }
}

