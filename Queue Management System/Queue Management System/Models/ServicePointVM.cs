using Queue_Management_System.Models;
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
     /*   public Data.AppUserVM User { get; set; }*/
        public AppUserVM AppUser { get; set; }

        [Display(Name = "Service Provider Id")]
        [Required]
        public int ServiceProviderId { get; set; }
    }
    public class ServicePointsList
    {
        public IEnumerable<ServicePointVM> Services { get; set; }
    }
}