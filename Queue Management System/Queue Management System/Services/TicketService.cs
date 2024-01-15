using Queue_Management_System.Models;

namespace Queue_Management_System.Services
{
    public interface ITicketService
    {
        TicketModel CreateTicket(string serviceId);

    }

    public class TicketService : ITicketService
    {
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

        private string GenerateTicketNumber()
        {
            return "T0012";
        }
    }
}