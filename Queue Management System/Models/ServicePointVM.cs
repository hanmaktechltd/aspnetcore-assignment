using Queue_Management_System.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Queue_Management_System.Models
{
    public class ServicePointVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public User User { get; set; }
        public int ServiceProviderId { get; set; }
    }
}
