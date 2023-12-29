
using Npgsql;
using Queue_Management_System.Models;

public interface ITicketService {

    Ticket SaveTicketToDatabase (CheckinViewModel checkInData);

    byte[] GenerateTicket(WaitingPageViewModel waitingPageData);

    //Ticket GetTicketById(int ticketId);

    public void MarkAsNoShow(int ticketId);

    public void MarkAsFinished(int ticketId);

    public void TransferTicket(int ticketId, int newServicePointId);

    public void UpdateTicketStatus(int ticketId, string status);

     //void UpdateServiceTime(NpgsqlConnection connection, int ticketId, DateTime serviceTime)







    


}