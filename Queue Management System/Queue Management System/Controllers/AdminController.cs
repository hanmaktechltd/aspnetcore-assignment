using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;
using Queue_Management_System.Models.Data;
using System.Diagnostics;

namespace Queue_Management_System.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public AdminController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Authenticated()
        {
            return View();
        }

        public IActionResult Login(string EmailAddress, string Password)
        {
            ViewBag.LoginStatus = "";

            if (ModelState.IsValid)
            {
                ViewBag.LoginStatus = "";

                var UserCheck = _dbContext.Administrator.FirstOrDefault
                    (a => a.EmailAddress == EmailAddress && a.Password == Password);

                if (UserCheck == null)
                {
                    ViewBag.LoginStatus = "Invalid Login. User not found";
                }
                else
                {
                    return RedirectToAction("Authenticated");
                }
            }
            return View();
        }
        public IActionResult ServicePoints()
        {
            var servicePoints = _dbContext.ServicePoints.ToList();
            return View(servicePoints);
        }

        public IActionResult ServiceProviders()
        {
            var serviceProviders = _dbContext.ServiceProviders.ToList();
            return View(serviceProviders);
        }

        public IActionResult AddServiceProvider()
        {
            return View();
        }


        public IActionResult AddServicePoint()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddServicePoint(ServicePoint servicePoint)
        {
            _dbContext.ServicePoints.Add(servicePoint);
            _dbContext.SaveChanges();
            return RedirectToAction("ServicePoints");
        }

        public IActionResult EditServicePoint(int id)
        {
            var servicePoint = _dbContext.ServicePoints.Find(id);
            if (servicePoint == null)
            {
                return NotFound();
            }
            return View(servicePoint);
        }

        [HttpPost]
        public IActionResult EditServicePoint(ServicePoint servicePoint)
        {
            _dbContext.ServicePoints.Update(servicePoint);
            _dbContext.SaveChanges();
            return RedirectToAction("ServicePoints");
        }

        public IActionResult DeleteServicePoint(int id)
        {
            var servicePoint = _dbContext.ServicePoints.Find(id);
            if (servicePoint == null)
            {
                return NotFound();
            }
            _dbContext.ServicePoints.Remove(servicePoint);
            _dbContext.SaveChanges();
            return RedirectToAction("ServicePoints");
        }

        [HttpPost]
        public IActionResult AddServiceProvider(Models.ServiceProvider serviceProvider)
        {
            _dbContext.ServiceProviders.Add(serviceProvider);
            _dbContext.SaveChanges();
            return RedirectToAction("ServiceProviders");
        }

        public IActionResult EditServiceProvider(int id)
        {
            var servicePoint = _dbContext.ServicePoints.Find(id);
            if (servicePoint == null)
            {
                return NotFound();
            }
            return View(servicePoint);
        }

        [HttpPost]
        public IActionResult EditServiceProvider(Models.ServiceProvider serviceProvider)
        {
            if (ModelState.IsValid)
            {
                _dbContext.ServiceProviders.Update(serviceProvider);
                _dbContext.SaveChanges();
            }
            return RedirectToAction("ServicePoints");
        }

        public IActionResult DeleteServiceProvider(int id)
        {
            var servicePoint = _dbContext.ServicePoints.Find(id);
            if (servicePoint == null)
            {
                return NotFound();
            }
            _dbContext.ServicePoints.Remove(servicePoint);
            _dbContext.SaveChanges();
            return RedirectToAction("ServicePoints");
        }

    }
}