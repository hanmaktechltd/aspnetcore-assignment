using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Queue_Management_System.Data
{
    public class ServicePoint
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("ServiceProviderId")]
        public User User { get; set; }
        public int ServiceProviderId { get; set; }
    }
}