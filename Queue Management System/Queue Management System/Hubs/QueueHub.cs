using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Queue_Management_System.Hubs
{
    public class QueueHub : Hub
    {
        public async Task UpdateQueue(dynamic queueData)
        {
            // Broadcast the queue update to all clients
            // await Clients.All.SendAsync("UpdateQueue", queueData);
        }
    }
}
