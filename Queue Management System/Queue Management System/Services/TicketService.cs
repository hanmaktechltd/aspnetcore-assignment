using Queue_Management_System.Models;

namespace Queue_Management_System.Services
{
    public interface ITicketService
    {
        TicketModel CreateTicket(string serviceId);

        void AddTicketToQueue(TicketModel ticket);

        TicketModel? GetTicketFromQueue(string serviceId);

        void AddTicketToNoShowTickets(TicketModel ticket);

        TicketModel? GetTicketFromNoShowTickets(string serviceId);

        void AddTicketToTicketsBeingCalled(TicketModel ticket, string servicePointId);

        //restore ticket queue from db incase of power loss

    }

    public class TicketService : ITicketService
    {
        private static readonly List<TicketModel> TicketsQueue = new List<TicketModel> ();
        
        private static readonly List<TicketModel> NoShowTicketsQueue = new List<TicketModel> ();

        private static readonly List<(TicketModel, string)> TicketsBeingCalled = new List<(TicketModel, string)> ();

        private string GenerateTicketNumber()
        {
            return "T0012";
        }
        
        public TicketModel CreateTicket(string serviceId)
        {
            string ticketNumber = this.GenerateTicketNumber();
            DateTime timePrinted = DateTime.Now;

            TicketModel ticket = new TicketModel(){
                TicketNumber = ticketNumber,
                ServiceId = serviceId,
                TimePrinted = timePrinted,
            };

            return ticket;
        }

        public void AddTicketToQueue(TicketModel ticket)
        {
            TicketsQueue.Add(ticket);
        }

        public TicketModel? GetTicketFromQueue(string serviceId)
        {
            TicketModel? ticket = TicketsQueue.Find(ticket => ticket.ServiceId == serviceId);
            TicketsQueue.Remove(ticket);
           
            return ticket;
        }

        public void AddTicketToNoShowTickets(TicketModel ticket)
        {
            NoShowTicketsQueue.Add(ticket);
        }

        public TicketModel? GetTicketFromNoShowTickets(string serviceId)
        {
            TicketModel? ticket = NoShowTicketsQueue.Find(ticket => ticket.ServiceId == serviceId);
            NoShowTicketsQueue.Remove(ticket);

            return ticket;
        }

        public void AddTicketToTicketsBeingCalled(TicketModel ticket, string servicePointId)
        {
            TicketsBeingCalled.Add((ticket, servicePointId));
        }

        
    }
}