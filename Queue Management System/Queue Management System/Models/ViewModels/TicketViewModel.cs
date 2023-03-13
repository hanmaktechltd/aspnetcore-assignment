using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models.ViewModels
{
    public class TicketViewModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string TicketNumber { get; set; }
        public string  Message { get; set; }
        
        public int CustomersInQueue { get; set; }
        public string servicePoint { get; set; }
    }
}
