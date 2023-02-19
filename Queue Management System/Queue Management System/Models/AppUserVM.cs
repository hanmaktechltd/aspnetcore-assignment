using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Queue_Management_System.Models
{
    public class AppUserVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public ServicePoint ServicePoint { get; set; }
        public int ServicePointId { get; set; }
    }
}
