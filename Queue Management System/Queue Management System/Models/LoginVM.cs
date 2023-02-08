using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class LoginVM
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberLogin { get; set; }
    }
}