namespace Queue_Management_System.Models
{
    public class ServicePointViewModel
    {
        public string? ServicePointID {get; set;}

        public string? ServiceDescription {get; set;}

        public TicketModel[]? TicketsInQueue {get; set;}

        public TicketModel[]? NoshowTickets {get; set;}

    }
}