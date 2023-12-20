using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;

namespace Queue_Management_System.Controllers
{
    public class QueueController : Controller
    {

        [HttpGet]
        public IActionResult CheckinPage()
        {
            return View();
        }



        // [HttpGet]
        // public IActionResult WaitingPage()
        // {
        //     return View();
        // }
     

        [Authorize, HttpGet]
        public IActionResult ServicePoint()
        {
            return View();
        }


        // [HttpPost]
        //  public string ProcessCheckIn(Ticket ticket) {

        //     return ticket.ServicePoint.ServicePointName;

        //  }

             
        [HttpPost] 
        public string WaitingPage(CheckinViewModel checkinData) {

           return checkinData.ServicePointName;
        } 



    }
}
