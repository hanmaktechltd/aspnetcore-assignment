using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Queue_Management_System.Data
{
    public class User
    {
        [Key]
        public int Id{ get; set; }
        public string Name{ get; set; }
        public string Password{ get; set; }
        public string Role{ get; set; }

        [ForeignKey("ServicePointId")]
        public ServicePoint ServicePoint { get; set; }
        public int ServicePointId { get; set; }
    }
}