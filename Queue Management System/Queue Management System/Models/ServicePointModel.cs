using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class ServicePointModel
    {
        [Key]
        public int id { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public DateTime datecreated { get; set; }
        public string? createdby { get; set; }
    }
}
