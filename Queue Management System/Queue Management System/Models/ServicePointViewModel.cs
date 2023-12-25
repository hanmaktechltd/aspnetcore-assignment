namespace Queue_Management_System.Models
{
    public class ServicePointViewModel
    {
        // Properties representing data related to service operations
        // Add properties as needed for your specific use case

        public string NextNumber { get; set; }
        public string RecalledNumber { get; set; }
        public string NoShowNumber { get; set; }
        public string FinishedNumber { get; set; }
        public string TransferredNumber { get; set; }
        public List<QueueItemModel> QueueItems { get; set; }

        // Other properties or operations related to service point view
    }

    public class QueueItemModel
    {
        // Properties representing a queue item
        public string TicketNumber { get; set; }
        public string CustomerName { get; set; }
        public DateTime CheckinTime { get; set; }
        // Add more properties as needed
    }
}
