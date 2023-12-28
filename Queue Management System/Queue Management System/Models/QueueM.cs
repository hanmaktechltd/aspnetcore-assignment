using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Queue_Management_System.Models
{
    public class QueueM
    {
        [Display(Name = "Queue Id")]
        public int? Id { get; set; }
        public ServicePointM ServicePoint { get; set; }

        [Display(Name = "Room Number")]
        public int ServicePointId { get; set; }
        public int Status { get; set; }

        [Display(Name = "Joined Queue At")]
        public DateTime CreatedAt { get; set; }

    }
}
