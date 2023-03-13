using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using Queue_Management_System.Data;
using Queue_Management_System.Models;
using Queue_Management_System.Models.ViewModels;
using Queue_Management_System.Repository;
using Queue_Management_System.Services;
using System.Data;

namespace Queue_Management_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly QueueRepository _repository;
        private readonly QueueService _queueService;

        public AdminController(
            RoleManager<IdentityRole> roleManager, QueueRepository repository, QueueService queueService, UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager
            )
        {
            _roleManager = roleManager;
            _repository = repository;
            _queueService = queueService;
            _userManager = userManager;
            _signInManager = signInManager;
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
                return BadRequest(new { error = "Validation error occured" });
            }
            IdentityRole identityRole = new IdentityRole
            {
                Name = role.Name,
            };
            var result = await _roleManager.CreateAsync(identityRole);
            if (result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(new { result });
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
            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Validation error occured" });
            }
            servicePoint.DateCreated= DateTime.Now.ToUniversalTime();
            var result= await  _repository.CreateServicePoint(servicePoint);
            await _repository.SaveChanges();
            return View();
            
        }
        public async Task<IActionResult> getReport()
        {
            var servicePoints = _repository.getServicePoints();
            List<SelectListItem> servicePointListItems = servicePoints.Select(sp => new SelectListItem
            {
                Value = sp.Id.ToString(),
                Text = sp.Name
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
            reportModel.AverageServiceTime = _queueService.GetAverageWaitTimeByServicePoint(filterModel);
            reportModel.AverageWaitTime = "90";
            report.Add(reportModel);
            
            var pdf = _queueService.GenerateAnalyticsReport(report);
            return File(pdf, "application/pdf", "analyticsreport.pdf");

        }
        [HttpGet]
        public IActionResult Register()
        {
            var roles = _roleManager.Roles.ToList();
            List<SelectListItem> RolesItems = roles.Select(role => new SelectListItem
            {
                Value = role.Name,
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
                    var user = new IdentityUser
                    {
                        UserName = registerDetails.Email,
                        Email = registerDetails.Email
                    };

                    var result = await _userManager.CreateAsync(user, registerDetails.Password);

                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, false);
                        //assign role
                        await AddUserToRole(registerDetails);
                        return CreatedAtAction("Register", new { Id = user.Id });
                    }
                    else
                    {
                        var err = result.Errors.First().Description;
                        return BadRequest(err);
                    }
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                var dsfsd = ex.Message;
                return BadRequest(ModelState);
            }
        }
        public async Task<IActionResult> AddUserToRole(RegisterViewModel registerDetails)
        {
            var user = _userManager.Users.SingleOrDefault(r => r.Email == registerDetails.Email);
            if (!await _userManager.IsInRoleAsync(user, registerDetails.Role))
            {
                IdentityResult result = await _userManager.AddToRoleAsync(user, registerDetails.Role);
                if (result.Succeeded)
                {
                }

            }
            return Ok();
        }

    }
}
