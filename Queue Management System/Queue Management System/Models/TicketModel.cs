namespace Queue_Management_System.Models
{
    public class TicketModel
    {
        public string TicketNumber {get; set;}
        
        public string ServiceId {get; set;}

        public string? ServicePointId {get; set;}

        public DateTime TimePrinted {get; set;}

        public bool? ShowedUp {get; set;}

        public DateTime? TimeShowedUp {get; set;}

        public DateTime? TimeFinished {get; set;}

        public string? ServiceProviderId {get; set;}  
    }
}