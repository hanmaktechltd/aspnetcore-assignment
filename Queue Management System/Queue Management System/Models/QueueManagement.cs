namespace Queue_Management_System.Models
{
    public class QueueManagement
    {
        public int QueueId { get; set; }
        public int CustomerId { get; set; }
        public int ServicePointId { get; set; }
        public int ProviderId { get; set; }
        public int TicketNumber { get; set; }
        public string Status { get; set; }
    }

}
