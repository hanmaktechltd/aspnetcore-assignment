using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class QueueVM
    {
        [Display(Name = "Queue Id")]
        public int? Id { get; set; }
        public ServicePointVM ServicePoint { get; set; }       
        public int ServicePointId { get; set; }     
        public int Status { get; set; }

        [Display(Name = "Joined Queue At")]
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CompletedAt { get; set; }
    }

    public class QueueVMList
    {
        [Display(Name = "Queue Id Number")]
        public int Id { get; set; }
        public QueueVM MyCurrentServingCustomerId { get; set; }
        public IEnumerable<QueueVM> WaitingCustomers { get; set; }

        [Display(Name = "Room Number")]
        public IEnumerable<QueueVM> CalledCustomers { get; set; }

        [Display(Name = "Joined Queue At")]
        public DateTime CreatedAt { get; set; }
    }
}