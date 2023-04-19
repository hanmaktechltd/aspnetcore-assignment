using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Queue_Management_System.Models;
using Queue_Management_System.Models.Data;

public class WaitingController : Controller
{
    private readonly ApplicationDbContext _dbContext;

    public WaitingController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public IActionResult Waiting(int servicePointId)
    {
        var waitingCustomers = _dbContext.Customers
                .Where(c => c.IsCalled && c.Status == "In Progress")
                .ToList();

        if (waitingCustomers == null)
        {
            TempData["error"] = "No customers currently on the queue";
        }

        return View(waitingCustomers);
    }

}
