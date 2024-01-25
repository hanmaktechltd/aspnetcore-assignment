using Queue_Management_System.Models;
using System.Linq;

namespace Queue_Management_System.Services
{
    public interface ITicketService
    {
        TicketModel CreateTicket(string serviceId);

        TicketModel TransferTicket(string ticketNumber, String serviceId);

        void AddTicketToQueue(TicketModel ticket);

        TicketModel? GetTicketFromQueue(string serviceId);

        void AddTicketToNoShowTickets(TicketModel ticket);

        TicketModel? GetTicketFromNoShowTickets(string serviceId);

        void AddTicketToTicketsBeingCalled(TicketModel ticket, string servicePointId);

        void RemoveTicketFromTicketsBeingCalled(string ticketNumber);

        IEnumerable<TicketModel> GetAllTicketsInQueueByServiceId(string serviceId);

        IEnumerable<TicketModel> GetAllNoShowTicketsInQueueByServiceId(string serviceId);

        IEnumerable<(TicketModel, string)> GetCalledTickets();

        //to do restore ticket queue from db incase of power loss

    }

    public class TicketService : ITicketService
    {
        private static readonly List<TicketModel> TicketsQueue = new List<TicketModel> ();
        
        private static readonly List<TicketModel> NoShowTicketsQueue = new List<TicketModel> ();

        private static readonly List<(TicketModel, string)> TicketsBeingCalled = new List<(TicketModel, string)> ();

        private string GenerateTicketNumber()
        {
            string startTime = "1/06/2010";
            var seconds = (DateTime.Now - DateTime.Parse(startTime)).TotalSeconds;
            return "TN" + Convert.ToString(Convert.ToInt32(seconds % 100000));
        }
        
        public TicketModel CreateTicket(string serviceId)
        {
            string ticketNumber = GenerateTicketNumber();
            DateTime timePrinted = DateTime.Now;

            TicketModel ticket = new TicketModel(){
                TicketNumber = ticketNumber,
                ServiceId = serviceId,
                TimePrinted = timePrinted,
            };

            return ticket;
        }

        public TicketModel TransferTicket(string ticketNumber, string serviceId)
        {
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

        public void RemoveTicketFromTicketsBeingCalled(string ticketNumber)
        {
            TicketsBeingCalled.RemoveAll(item => item.Item1.TicketNumber == ticketNumber);
        }

        public IEnumerable<TicketModel> GetAllTicketsInQueueByServiceId(string serviceId)
        {
            return TicketsQueue.FindAll(ticket => ticket.ServiceId == serviceId);
        }

         public IEnumerable<TicketModel> GetAllNoShowTicketsInQueueByServiceId(string serviceId)
        {
            return NoShowTicketsQueue.FindAll(ticket => ticket.ServiceId == serviceId);
        }

        public IEnumerable<(TicketModel, string)> GetCalledTickets()
        {
            return TicketsBeingCalled;
        }
        
    }
}