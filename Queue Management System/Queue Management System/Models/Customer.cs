using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class Customer
    {
        public int Id { get; set; }
        [Required]
        public string ? Name { get; set; }
        public string ServiceType { get; set; }
        public DateTime CheckInTime { get; set; }
        public Customer()
        {
            Name = string.Empty;
            ServiceType = string.Empty;
            CheckInTime = DateTime.MinValue;
        }
    }
}
