using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class ServicePoint
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public int ServiceProviderId { get; set; }

        public ICollection<ServiceProvider> ServiceProviders { get; set; }
    }
}
