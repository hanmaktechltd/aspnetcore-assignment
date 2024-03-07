using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Queue_Management_System.Models
{
    public class WaitingModel
    {
        
        [Required] public int Id { get; set; }
        public int? TicketNumber { get; set; }
        public int? ServicePoint { get; set; }
        [ForeignKey("ServicePoint")]
        public ServicePoint Service { get; set; }
        public string? ServicePointName { get; set; }
        public void OnGet(int? ticketNumber, int? servicePoint, string? servicePointName)
        {
            TicketNumber = ticketNumber;
            ServicePoint = servicePoint;
            ServicePointName = servicePointName;
        }
    }
}
