
using Queue_Management_System.Models;

public interface ITicketService {

    Ticket SaveTicketToDatabase (string serviceType);

    byte[] GenerateTicket(WaitingPageViewModel waitingPageData);

    


}