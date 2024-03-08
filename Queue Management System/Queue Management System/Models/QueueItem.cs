using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Queue_Management_System.Models
{
    public class QueueItem
    {
        public int Id { get; set; }
        [Required]
        public int? TicketNumber { get; set; }
        public int? ServicePoint { get; set; }
        [ForeignKey("ServicePoint")]
        public ServicePoint Service { get; set; }
        public string? ServicepointName { get; set; }
        public bool NoShow { get; set; }
        public bool Finished { get; set; }
        public bool IsCalled { get; set; }

    }
}
