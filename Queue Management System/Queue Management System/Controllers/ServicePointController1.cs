using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;
using System;

namespace Queue_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicePointController1 : ControllerBase
    {
        private readonly AppDbContext _context;
        public ServicePointController1(AppDbContext context)
        {
            _context = context;
        }
       /* public IActionResult Index() 
        {
            var servicepoints = _context.ServicePoints.ToList();
            return View(servicePoints);
        }*/
        /*[HttpGet("{id}/next")]
        public IActionResult GetNextNumber(int id)
        {
            var nextNumber = _context.QueueItems
                .Where(q => q.ServicePointId == id)
                .OrderBy(q => q.Id)
                .Select(q => q.TicketNumber)
                .FirstOrDefault();

            if (nextNumber == null)
                return NotFound();

            return Ok(nextNumber);
        }*/
    }
}
