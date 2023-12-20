
namespace Queue_Management_System.Models;
public class ServicePoint {

    public int ServicePointID { get; set; }
    public string ServicePointName { get; set; }
    public string Description { get; set; }

    public ICollection<Ticket> Tickets { get; set; }
}