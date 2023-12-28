using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class ServiceProviderM
    {
        
        

        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }
        public string Role { get; set; }


        [Display(Name = "Service Point Id")]
        public int ServicepointId { get; set; }
    }
}
