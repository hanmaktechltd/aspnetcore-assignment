using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class Admin
    {
        [Required(ErrorMessage = "Please Enter Your Username!")]
        [Display(Name = "Enter Username :")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please Enter Your Password!")]
        [Display(Name = "Enter Password :")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
