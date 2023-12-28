using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Net;


namespace Queue_Management_System.Models
{
    public class QueueMList
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ServicePointId")]
        public ServicePoint ServicePoint { get; set; }
        public int ServicePointId { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}
