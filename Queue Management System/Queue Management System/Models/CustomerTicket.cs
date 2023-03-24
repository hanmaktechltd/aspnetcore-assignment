using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Queue_Management_System.Models
{
    public class CustomerTicket
    {
        public int Id { get; set; }
        public int ServicePointId { get; set; }
        public ServicePoint ServicePoint { get; set; }
        public int ServiceProviderId { get; set; }
        public ServiceProvider ServiceProvider { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime? CallTime { get; set; }
        public DateTime? StartServiceTime { get; set; }
        public DateTime? EndServiceTime { get; set; }

        public string Status { get; set; }
        public bool NoShow { get; set; }
        public bool Completed { get; set; }

        public bool IsCalled { get; set; }
    }

}
