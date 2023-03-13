using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models.ViewModels
{
    public class RoleViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
