using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Queue_Management_System.Models
{
    public class UserM
    {

        [Required]
        public string Name { get; set; }
        [Required]
        public string Role { get; set; }

        [ForeignKey("ServicePointId")]
        public ServicePointM ServicePoint { get; set; }
        public int ServicePointId { get; set; }
    }
}
