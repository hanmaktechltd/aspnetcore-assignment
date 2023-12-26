using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class ServiceProviderConfiguration
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter Service Provider Name.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter Service Provider Password.")]
        public string Password { get; set; }
    }
}
