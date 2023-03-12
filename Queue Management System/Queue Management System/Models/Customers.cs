namespace Queue_Management_System.Models
{
    public class Customers
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public ServicePointModel ServicePoint { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime TimeOut { get; set; }
        public string TicketNumber { get; set; }
        public string Status { get; set; }
    }
}
