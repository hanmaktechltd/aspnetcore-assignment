namespace Queue_Management_System.Models
{
    public class WaitingPageModel
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; }
        public string ServicePoint { get; set; }
        public string CustomerName { get; set; }
        public DateTime CheckinTime { get; set; }

        // Constructor
        public WaitingPageModel( string ticketNumber, string servicePoint, string customerName, DateTime checkinTime)
        {
            TicketNumber = ticketNumber;
            ServicePoint = servicePoint;
            CustomerName = customerName;
            CheckinTime = checkinTime;
        }
    }
}
