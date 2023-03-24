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
                .Where(c => !c.IsCalled && c.Status == "Waiting")
                .ToList();

        if (waitingCustomers == null)
        {
            return View("Empty");
        }

        return View(waitingCustomers);
    }

}
