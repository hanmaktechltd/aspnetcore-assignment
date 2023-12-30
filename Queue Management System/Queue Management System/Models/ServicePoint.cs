
using System.ComponentModel.DataAnnotations;
using Queue_Management_System.Models;
public class ServicePoint {

    public int ServicePointId { get; set; }
    public string ServicePointName { get; set; }
    
    [Required(ErrorMessage = "A Description is required.")]
    public string Description { get; set; }

    public ICollection<ServiceProvider>? ServiceProviders { get; set; }
    
    public ICollection<Ticket>? Tickets { get; set; }
}