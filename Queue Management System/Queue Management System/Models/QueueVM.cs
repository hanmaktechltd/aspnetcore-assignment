using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class QueueVM
    {
        public int Id { get; set; }
        public ServicePointVM ServicePointId { get; set; } 
        public DateTime CreatedAt { get; set; }
    }

    public class QueueVMList
    {
        [Display(Name = "Queue Id Number")]
        public int Id { get; set; }
        public int ServicePointCount { get; set; }
        public QueueVM MyCurrentServingCustomerId { get; set; }
        public IEnumerable<QueueVM> WaitingCustomers { get; set; }
        public IEnumerable<ServicePointVM> Services { get; set; }

        [Display(Name = "Joined Queue At")]
        public DateTime CreatedAt { get; set; }
    }
}