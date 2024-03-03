using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class ServiceProvider
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Service { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
