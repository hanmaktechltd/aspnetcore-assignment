using Microsoft.AspNetCore.Identity;

namespace Queue_Management_System.Models
{
    public class ApplicationUser : IdentityUser
    {
        // You can add custom properties here if needed
        public string CustomProperty { get; set; }
    }
}
