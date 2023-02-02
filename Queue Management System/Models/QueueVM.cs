using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace Queue_Management_System.Models
{
    public class QueueVM
    {
        public int Id { get; set; }
        public ServicePointVM ServicePoint { get; set; }
        public int ServicePointId { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}
