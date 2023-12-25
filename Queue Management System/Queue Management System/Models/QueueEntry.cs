namespace Queue_Management_System.Models
{
    public class QueueEntry
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; }
        public string ServicePoint { get; set; }
        public string CustomerName { get; set; }
        public DateTime CheckinTime { get; set; }
    }

    public QueueEntry()
    {
       
    }
}
