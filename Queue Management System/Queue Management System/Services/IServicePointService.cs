
using Queue_Management_System.Models;

public interface IServicePointService {

   public List<Ticket> findTicketsPerServicePoint (int servicePointId);

   public List<ServicePoint> GetServicePoints();

   public ServicePoint GetServicePointById(int servicePointId);

   public Ticket GetCurrentTicketPerServicePoint (int servicePointId);

   public Ticket GetNextTicketPerServicePoint (int servicePointId);

}