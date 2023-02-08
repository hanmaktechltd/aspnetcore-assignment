using Queue_Management_System.Data;
using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class ServicePointVM
    {
        [Display(Name = "Service Point Id")]
        public int Id { get; set; }

        [Display(Name = "Service Point Name")]
        [Required]
        public string Name { get; set; }
        public User User { get; set; }

        [Display(Name = "Service Provider Id")]
        [Required]
        public int ServiceProviderId { get; set; }
    }
}