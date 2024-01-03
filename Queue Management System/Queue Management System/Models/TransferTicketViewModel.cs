
using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models;

public class TransferTicketViewModel {

    public int TicketId {get; set;}

    public int DestinationServicePointId {get; set;}

    public int OriginServicePointId {get; set;}

    public List <ServicePoint>? AvailableServicePoints {get; set;}

    public int CurrentServiceProviderId {get; set;}
}