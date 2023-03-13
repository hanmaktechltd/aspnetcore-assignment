using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class ServicePointModel
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime DateCreated { get; set; }
        public string? CreatedBy { get; set; }
    }
}
