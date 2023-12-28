using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class LoginM
    {
        

        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }

    }
}
