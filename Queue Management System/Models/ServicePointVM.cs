using Queue_Management_System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Queue_Management_System.Models
{
    public class ServicePointVM
    {
        public int Id { get; set; }

        [Display(Name = "Service Point Name")]
        [Required]
        public string Name { get; set; }
        public User User { get; set; }

        [Display(Name = "Service Provider Id")]
        [Required]
        public int ServiceProviderId { get; set; }
        public string ServiceName { get; set; }

   /*     public QueueVM CreatedAt { get; set; }
        public QueueVM UpdatedAt { get; set; }
        public QueueVM CompletedAt { get; set; }*/
    }
}
