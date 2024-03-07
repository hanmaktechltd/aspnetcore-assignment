using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;
using System.Linq;
namespace Queue_Management_System.Controllers
{
    public class CheckIn : Controller
    {
        private readonly AppDbContext _context;

        public CheckIn(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CheckIn1(Customer model)
        {
            /*if (!ModelState.IsValid)
            {
                return View(model);
            }*/

            model.CheckInTime = DateTime.UtcNow;
            model.Status = "Waiting";
            _context.customers.Add(model);           
            _context.SaveChanges();
            var waitModel = new WaitingModel();
            UpdateWaitingPage(waitModel);
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult UpdateWaitingPage(WaitingModel model1)
        {
            var customerInwaiting = _context.customers.ToList();
            try
            {
                foreach (var client in customerInwaiting)
                {
                    if ((client != null) && (client.Status == "Waiting"))
                    {
                        var servicePoint = _context.ServicePoints.Where(sp => sp.Status == "Open").ToList();
                        if (servicePoint != null)
                        {
                            foreach (var queue in servicePoint)
                            {
                                model1.TicketNumber = client.Id;
                                model1.ServicePoint = queue.Id;
                                model1.ServicePointName = queue.Name;
                                _context.waitingModels.Add(model1);
                                _context.SaveChanges();
                                //update Service Point Status
                                queue.Status = "Busy";
                                _context.ServicePoints.Update(queue);
                                _context.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return View();
        }
    }
}
