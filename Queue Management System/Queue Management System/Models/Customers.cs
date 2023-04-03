using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class Customers
    {
        [Key]
        public int id { get; set; } 
        public string name { get; set; }
        public ServicePointModel servicepoint { get; set; }
        public DateTime timein { get; set; }
        public DateTime timeservicestarted { get; set; }
        public DateTime timeout { get; set; }
        public string ticketnumber { get; set; }
        public string status { get; set; }
    }
}
