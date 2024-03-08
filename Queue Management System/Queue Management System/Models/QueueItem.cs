using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class QueueItem
    {
        public int Id { get; set; }
        [Required]
        public string TicketNumber { get; set; }
        public int ServicePointId { get; set; }
        //public ServicePoint ServicePoint { get; set; }
        public bool NoShow { get; set; }
        public bool Finished { get; set; }

    }
}
