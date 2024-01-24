using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class ServiceModel
    {
        [Required(ErrorMessage = "Service ID is required")]
        public string Id { get; set; }
    
        [Required(ErrorMessage = "Service Description is required")]
        public string Description { get; set; }

    }
    
}