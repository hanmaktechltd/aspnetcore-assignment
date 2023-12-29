namespace Queue_Management_System.Models
{
    public class ServiceStatistics
    {
        public int FinishedCount { get; set; }
        public int ServicePointId { get; set; }
        public double AverageSeconds { get; set; }
        public int Id { get; set; }
        public string TicketNumber { get; set; }
        public string ServicePoint { get; set; }
        public string CustomerName { get; set; }
        public int NoShowServicePointId { get; set; }
    }
}
