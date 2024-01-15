using Queue_Management_System.Models;

namespace Queue_Management_System.Repositories
{
    public interface ITicketRepository
    {
        void AddTicket(TicketModel ticket);

        TicketModel GetNextUnservedTicketByServiceId(string serviceId);

        IEnumerable<TicketModel> GetUnservedTicketsByServiceID(string serviceId);

        void UpdateTicket(TicketModel ticket);
    }
}