using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class ServiceProviderModel
    {
        [Required(ErrorMessage = "Service Provider ID is required")]
        public string Id {get; set;}

        [Required(ErrorMessage = "Service Provider Name is required")]
        public string Name {get; set;}

        [Required(ErrorMessage = "Service Provider Email Address is required")]
        [EmailAddress]
        public string Email {get; set;}

        [Required(ErrorMessage = "Service Provider Password is required")]
        [StringLength(100, ErrorMessage = "Password must be atleast 6 characters long", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password {get; set;}

        [Required(ErrorMessage = "Service Provider Confirmation Password is required")]
        [Compare("Password", ErrorMessage = "Password and Confirmation Password must match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword{get; set;}

        public bool IsAdmin{get; set;}





    }
}