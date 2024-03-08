using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;
//using System.Net;

namespace Queue_Management_System.Controllers
{
    public class ServicePointController : Controller
    {
        private readonly AppDbContext _context;
        public ServicePointController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var servicepoints = _context.ServicePoints.ToList();
            return View(servicepoints);
        }        
        [HttpPost]
        public IActionResult Create(ServicePoint model)
        {
            //Find the last record and increment it by 1
            int lastId=_context.ServicePoints.OrderByDescending(x=>x.Id).Select(x=>x.Id).FirstOrDefault();
            model.Id = lastId + 1;
            _context.ServicePoints.Add(model);
            _context.SaveChanges();
            return RedirectToAction("ServicePoints", "Home");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var servicepoint = _context.ServicePoints.FirstOrDefault(x => x.Id == id);
            if (servicepoint == null)
            {
                return NotFound(); 
            }
            return View(servicepoint);
        }
        [HttpPost]
        public IActionResult Edit(ServicePoint model)
        {
            _context.ServicePoints.Update(model);
            _context.SaveChanges();
            return RedirectToAction("ServicePoints", "Home");
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var servicepoint = _context.ServicePoints.FirstOrDefault(x => x.Id == id);
            if (servicepoint != null)
            {
                _context.ServicePoints.Remove(servicepoint);
                _context.SaveChanges();
            }
            return RedirectToAction("ServicePoints", "Home");
        }

    }
}
