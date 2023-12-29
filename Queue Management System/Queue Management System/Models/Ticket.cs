
namespace Queue_Management_System.Models;
public class Ticket {

    public int TicketId {get; set;}

    public DateTime IssueTime {get; set;}


    public string Status {get; set;}

    public DateTime ServiceTime {get; set;}

    public int ServicePointId { get; set; }
    public ServicePoint ServicePoint { get; set; }
    

}