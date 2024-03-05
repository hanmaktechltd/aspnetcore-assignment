using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Queue_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicePointController : ControllerBase
    {
       /* private readonly AppDbContext _context;

        public ServicePointController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}/next")]
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
