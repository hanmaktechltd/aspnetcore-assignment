using Queue_Management_System.Models;

namespace Queue_Management_System.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public int QueueNumber { get; set; }
        public int? ServicePointId { get; set; }
        public ServicePoint ServicePoint { get; set; }
    }
}
