
namespace Queue_Management_System.Models;
public class Ticket {

    public int TicketId {get; set;}
    public int TicketNumber {get; set;}

    public int ServicePointId { get; set; } 
    public DateTime IssueTime {get; set;}

    public DateTime ServiceTime {get; set;}

    // public ServicePoint ServicePoint { get; set; }
    
    public string ServicePointName {get; set;}

}