namespace Queue_Management_System.Models
{
    public class WaitingModel
    {
        public int TicketNumber { get; set; }
        public string ServicePoint { get; set; }
        public void OnGet(int ticketNumber, string servicePoint)
        {
            TicketNumber = ticketNumber;
            ServicePoint = servicePoint;
        }
    }
}
