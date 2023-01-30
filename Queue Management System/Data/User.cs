using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Data
{
    public class User
    {
        [Key]
        public int Id{ get; set; }
        public string Name{ get; set; }
        public int Password{ get; set; }
    }
}
