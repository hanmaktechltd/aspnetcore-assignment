using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class TransferView
    {
        public CustomerTicket Customers { get; set; }
        public IEnumerable<ServicePoint> ServicePoints { get; set; }
        public int DestinationServicePointId { get; set; }
    }
}
