namespace Queue_Management_System.Models
{
    public class ServedCustomers
    {
        public int FinishedId { get; set; }
        public string CustomerName { get; set; }
        public string TicketNumber { get; set; }
        public int ServicePointId { get; set; }
        public string ServicePoint { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime MarkedTime { get; set; }
    }
}
