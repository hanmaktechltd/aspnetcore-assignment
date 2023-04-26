using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Queue_Management_System.Models;
using Queue_Management_System.Services;

public class WaitingController : Controller
{
    private readonly ICheckInRepository _checkInRepository;

    public WaitingController(ICheckInRepository checkInRepository)
    {
        _checkInRepository = checkInRepository;
    }
    public async Task<IActionResult> Waiting(int servicePointId)
    {
        var waitingCustomers = await _checkInRepository.Waiting();

        if (waitingCustomers is null)
        {
            TempData["error"] = "No customer called";
        }

        return View(waitingCustomers);
    }

}
