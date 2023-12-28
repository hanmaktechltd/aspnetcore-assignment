using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Queue_Management_System.Models
{
    public class QueueM
    {
        [Display(Name = "Ticket Id")]
        public int? Id { get; set; }
        

        [Display(Name = "Room Number")]
        public int ServicePointId { get; set; }
        public int Status { get; set; }

        [Display(Name = "Joined Queue At")]
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CompletedAt { get; set; }

    }
}
