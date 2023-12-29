using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Net;


namespace Queue_Management_System.Models
{
    public class QueueMList
    {
        [Key]
        [Display(Name = "Queue Id Number")]
        public int Id { get; set; }
        public int? ServicePointCount { get; set; }
        
        public QueueM CurrentServingCustomerId { get; set; }
        public IEnumerable<QueueM> WaitingCustomers { get; set; }
        public IEnumerable<ServicePointM> Services { get; set; }
        [Display(Name = "Joined Queue At")]
        public DateTime JoinedAt { get; set; }
    }
}
