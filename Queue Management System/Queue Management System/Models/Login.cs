using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class Login
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

    }
}
