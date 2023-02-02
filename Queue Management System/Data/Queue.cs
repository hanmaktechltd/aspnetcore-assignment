using Queue_Management_System.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Queue_Management_System.Data
{
    public class Queue
    {
        public int Id { get; set; }
        public ServicePoint ServicePoint { get; set; }
        public int ServicePointId { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}
