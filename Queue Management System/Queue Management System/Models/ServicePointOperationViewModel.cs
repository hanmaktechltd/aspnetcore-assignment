namespace Queue_Management_System.Models
{
    public class ServicePointOperationViewModel
    {
        public string? ServicePointId {get; set;}

        public string? ServiceDescription {get; set;}

        public IEnumerable<TicketModel>? TicketsInQueue {get; set;}

        public IEnumerable<TicketModel>? NoShowTickets {get; set;}

        public IEnumerable<ServiceModel>? Services {get; set;}

    }
}