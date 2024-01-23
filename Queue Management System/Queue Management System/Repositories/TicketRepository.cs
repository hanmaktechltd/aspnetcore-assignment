using Queue_Management_System.Models;

namespace Queue_Management_System.Repositories
{
    public interface ITicketRepository
    {
        void AddTicket(TicketModel ticket);

        TicketModel GetUnservedTicketByTicketNumber(string ticketNumber);

        //IEnumerable<TicketModel> GetUnservedTicketsByServiceID(string serviceId);

        //void UpdateTicket(TicketModel ticket);

        void MarkTicketAsShowedUp(string ticketNumber, DateTime showUpTime);

        void SetTicketFinishTime(string ticketNumber, DateTime finishTimme);
    }

    public class MockTicketRepository : ITicketRepository
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

    }
}

