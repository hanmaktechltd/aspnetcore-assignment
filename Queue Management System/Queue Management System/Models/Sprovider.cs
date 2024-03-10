using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class Sprovider
    {
        [Key]
        public long id { get; set; }
        public string  Name { get; set; }
        public string? Status { get; set; }
    }
}
