using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Queue_Management_System.Models;
using Queue_Management_System.Models.ViewModels;
using Queue_Management_System.Repository;
using Queue_Management_System.Services;
using System.Data;

namespace Queue_Management_System.Controllers
{
    [Authorize(Roles  = "Admin")]
    public class AdminController : Controller
    {
        private readonly QueueRepository _repository;
        private readonly QueueService _queueService;

        public AdminController( QueueRepository repository, QueueService queueService
            )
        {
            _repository = repository;
            _queueService = queueService;
        }


        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }
        [AllowAnonymous]
        public async Task<IActionResult> addRoles()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> addRoles(RoleViewModel role)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, $"Validation error occured");

                return View();
            }
            IdentityRole identityRole = new IdentityRole
            {
                Name = role.Name,
            };
            var result = await _repository.AddRole(identityRole);
            if (result)
            {
                ModelState.AddModelError(string.Empty, $"Added Successfully");
                return View();
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"Failed to add role");
                return View();
            }
        }
        //add services
        public async Task<IActionResult> addServices()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> addServices(ServicePointModel servicePoint)
        {
            servicePoint.datecreated= DateTime.Now.ToUniversalTime();
            servicePoint.createdby = HttpContext.Session.GetString("userName");
            var result= await  _repository.CreateServicePoint(servicePoint);
            TempData["Message"] = "Added successfully";
            return View();
            
        }
        public async Task<IActionResult> getReport()
        {
            var servicePoints = _repository.getServicePoints();
            List<SelectListItem> servicePointListItems = servicePoints.Select(sp => new SelectListItem
            {
                Value = sp.id.ToString(),
                Text = sp.name
            }).ToList();

            ViewBag.ServicePoints = servicePointListItems;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> getReport(FilterModel filterModel)
        {
            List<ReportModel> report = new List<ReportModel>();
            ReportModel reportModel = new ReportModel();
            reportModel.CustomersServed = _queueService.GetServedCustomersByServicePoint(filterModel);
            reportModel.AverageWaitTime = _queueService.GetAverageWaitTimeByServicePoint(filterModel);
            reportModel.AverageServiceTime = _queueService.GetAverageServiceTimeByServicePoint(filterModel);
            report.Add(reportModel);
            
            var pdf = _queueService.GenerateAnalyticsReport(report);
            return File(pdf, "application/pdf", "analyticsreport.pdf");

        }
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var roles = await _repository.getRoles();
            List<SelectListItem> RolesItems = roles.Select(role => new SelectListItem
            {
                Value = role.Id,
                Text = role.Name
            }).ToList();

            ViewBag.RolesItems = RolesItems;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerDetails)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DateTime date = DateTime.Now;

                    var user = await _repository.getUserByEmail(registerDetails.Email);
                    if (user.Email == null)
                    {
                        registerDetails.Password = await _queueService.HashPassword(registerDetails.Password);
                        var result = await _repository.createUser(registerDetails);
                        if (result)
                        {
                            //assign role
                            await _repository.AddUserToRole(registerDetails.Email, registerDetails.Role);
                            return RedirectToAction("Dashboard");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Could not create user. please try again");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"User with email {registerDetails.Email} already exists");

                    }


                }
                var roles = await _repository.getRoles();
                List<SelectListItem> RoleItems = roles.Select(role => new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name
                }).ToList();
                ViewBag.roles = RoleItems;
                return View();
            }
            catch (Exception ex)
            {
                var roles = await _repository.getRoles();
                List<SelectListItem> RoleItems = roles.Select(role => new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name
                }).ToList();
                ViewBag.roles = RoleItems;
                var dsfsd = ex.Message;
                return View();
            }
        }
 

    }
}
