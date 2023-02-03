using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Xml.Linq;

namespace Queue_Management_System.Models
{
    public class QueueVM
    {
        [Display(Name = "Customer Id")]
        public int Id { get; set; }
        public ServicePointVM ServicePoint { get; set; }
        public int ServicePointId { get; set; }
        public int Status { get; set; }
        [Display(Name = "Joined Queue At")]
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}
