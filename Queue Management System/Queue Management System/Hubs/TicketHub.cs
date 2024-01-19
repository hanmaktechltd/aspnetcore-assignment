// TicketHub.cs
using Microsoft.AspNetCore.SignalR;

namespace Queue_Management_System.Hubs;

public class TicketHub : Hub
{
    public async Task SendCalledTicket(int ticketId, string servicePoint)
    {
        await Clients.All.SendAsync("ReceiveCalledTicket", ticketId, servicePoint);
    }
}
