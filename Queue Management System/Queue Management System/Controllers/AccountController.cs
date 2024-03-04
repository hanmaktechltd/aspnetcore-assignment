using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Queue_Management_System.Constants;
using Queue_Management_System.Data;
using Queue_Management_System.Models;
using Queue_Management_System.Utils;

namespace Queue_Management_System.Controllers
{
    public class AccountController : Controller
    {
        private readonly QueueDbContext _dbContext;
        public AccountController(QueueDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind("Username,Password")] Login login)
        {

            if (string.IsNullOrEmpty(login.Username))
            {
                ModelState.AddModelError("Username", "Kindly provide username");
                return View();
            }
            if (string.IsNullOrEmpty(login.Password))
            {
                ModelState.AddModelError("Password", "Kindly provide password");
                return View();
            }
            var user = _dbContext.UserAccounts.FirstOrDefault(u => u.Username.Equals(login.Username));
            if (user == null)
            {
                ModelState.AddModelError("Username", "Invalid User Name credentials");
                return View();

            }


            var pass = Decryptor.Decript_String(login.Password);
            if (!pass.Equals(user.Password))
            {
                ModelState.AddModelError("Password", "Wrong Password.");
                return View();
            }
            else { 

                    if (ModelState.IsValid)
                    {
                        try
                        {
                        HttpContext.Session.SetString(StrValues.LoggedInUser, user.Username);

                        HttpContext.Session.SetString(StrValues.Department, user.Department);
                        return RedirectToAction("ServicePoint", "Queue", new { area = "" });


                        }
                        catch (Exception ex)
                        {
                        }
                    } 
                }
            return View(login);
        }

    }
}
