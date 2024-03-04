using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Queue_Management_System.Pages
{
    public class WaitingpageModel : PageModel
    {
        public int TicketNumber { get; set; }
        public string ServicePoint { get; set; }
        public void OnGet(int ticketNumber, string servicePoint)
        {
            TicketNumber = ticketNumber;
            ServicePoint = servicePoint;
        }
    }
}
