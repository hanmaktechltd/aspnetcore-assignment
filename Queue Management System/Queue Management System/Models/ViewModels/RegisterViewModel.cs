using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Key]
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Role { get; set; }
    }
}
