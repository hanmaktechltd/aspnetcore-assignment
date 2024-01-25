using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class ServicePointModel
    {
        [Required(ErrorMessage = "Service Point ID is required")]
        public string Id {get; set;}

        public string? Description {get; set;}
        public string? ServiceProviderId {get; set;}
    }
}