using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class Spoints
    {
        [Key]
        public long id { get; set; }
        public string Counter { get; set; }
        public string ServiceProvider { get; set; }
        public Boolean Status { get; set; }
    }
}
