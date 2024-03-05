using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class QueueItem
    {
        public int Id { get; set; }
        [Required]
        public int ServicePointId { get; set; }
        public string TicketNumber { get; set; }
    }
}
