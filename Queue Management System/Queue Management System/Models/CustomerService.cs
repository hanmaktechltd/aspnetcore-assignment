using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class CustomerService
    {
        [Key]
        public long TicketNumber { get; set; }
        public string ServiceRequested { get; set; }
        public string Status { get; set; }
        public DateTime serviceDate { get; set; }    
    }
}
