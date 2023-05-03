using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class ServiceProvider
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Password { get; set; }
        public int ServicePointId { get; set; }
        public ServicePoint ServicePoint { get; set; }
    }
}
