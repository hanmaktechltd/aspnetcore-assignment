
namespace Queue_Management_System.Models;
public class WaitingPageViewModel {

    
    public List<Ticket>? Tickets {get; set;}
    public int TicketNumber {get; set;}

    public string ServicePointName { get; set; }

    public DateTime IssueTime {get; set;}
}