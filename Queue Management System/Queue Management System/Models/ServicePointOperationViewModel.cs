namespace Queue_Management_System.Models
{
    public class ServicePointOperationViewModel
    {
        public string? ServicePointId {get; set;}

        public string? ServiceDescription {get; set;}

        public TicketModel[]? TicketsInQueue {get; set;}

        public TicketModel[]? NoshowTickets {get; set;}

        public IEnumerable<ServiceModel>? Services {get; set;}

    }
}