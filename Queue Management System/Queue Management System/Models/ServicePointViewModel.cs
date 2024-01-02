using Queue_Management_System.Models;

public class ServicePointViewModel {

    public int CurrentTicketNumber {get; set;}

    public DateTime CurrentTicketIssueTime {get; set;}
    public ICollection<Ticket>? AllTickets { get; set; }

    public Boolean HasTickets {get; set;}

    public int CurrentServicePointId {get; set;}

    public int CurrentTicketIndex {get; set;}

    public int TicketCount {get; set;}

    public ICollection<Ticket>? NotServedTickets { get; set; }

    public ICollection<ServiceProvider> AllServiceProviders {get; set;}

    public int CurrentServiceProviderId {get; set;}

    
}
