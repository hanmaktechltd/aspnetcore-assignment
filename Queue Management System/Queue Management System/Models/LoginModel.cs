using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Please enter an email address")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email {get; set;}

        [Required(ErrorMessage = "Please enter password")]
        public string Password {get; set;}
    }
}