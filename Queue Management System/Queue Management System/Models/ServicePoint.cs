using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class ServicePoint
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the Service Point Name.")]
        public string ServicePointName { get; set; }

        [Required(ErrorMessage = "Please enter the person serving at this service point.")]
        public string ServedBy { get; set; }
    }
}
