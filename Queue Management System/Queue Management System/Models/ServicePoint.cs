using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class ServicePoint
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
