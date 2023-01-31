using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Data
{
    public class User
    {
        [Key]
        public int Id{ get; set; }
        public string Name{ get; set; }
        public string Password{ get; set; }
        public string Role{ get; set; }
    }
}
