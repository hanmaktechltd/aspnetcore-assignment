using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Core.Types;
using Queue_Management_System.Models.ViewModels;
using Queue_Management_System.Repository;
using Queue_Management_System.Services;
using System.Data;
using System.Net;
using System.Security.Claims;

namespace Queue_Management_System.Controllers
{

    public class AccountController : Controller
    {
        private readonly string conString;
        private readonly QueueRepository _repository;
        private readonly QueueService _queueService;

        public AccountController(
            IConfiguration configuration,
            QueueRepository repository,
            QueueService queueService
            )
        {
            conString = configuration.GetConnectionString("DefaultConnection");
            _repository = repository;
            _queueService= queueService;
        }

        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginDetails, string? returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    loginDetails.Password = await _queueService.HashPassword(loginDetails.Password);
                    var result = await _repository.checkPassword(loginDetails);//_signInManager.PasswordSignInAsync(loginDetails.Email, loginDetails.Password, loginDetails.RememberMe, lockoutOnFailure: false);
                    if (result)
                    {
                        var user = await _repository.getUserByEmail(loginDetails.Email);
                        var roles = await _repository.getUserRoles(user);//_userManager.GetRolesAsync(user);
                        var claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.Name, user.Email));
                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role.Name));
                            HttpContext.Session.SetString("Role", role.Name);
                        }

                        //Initialize a new instance of the ClaimsIdentity with the claims and authentication scheme
                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        //Initialize a new instance of the ClaimsPrincipal with ClaimsIdentity
                        var principal = new ClaimsPrincipal(identity);
                        //SignInAsync is a Extension method for Sign in a principal for the specified scheme.
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                            principal, new AuthenticationProperties() { IsPersistent = true });
                        

             

                        HttpContext.Session.SetString("userName", loginDetails.Email);

                        if (returnUrl == null)
                        {
                            if (HttpContext.Session.GetString("Role")=="Admin")
                            {
                                return RedirectToAction("Dashboard", "Admin");
                            }
                            if (HttpContext.Session.GetString("Role") == "ServiceProvider")
                            {
                                return RedirectToAction("SelectServicePoint", "Account");
                            }
                            return RedirectToAction("Checkinpage", "Queue");
                        }
                        return Redirect(returnUrl);
                    }

                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return View(loginDetails);
                    }
                }
                return View(loginDetails);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occured");
                return View();
            }
        }

        public async Task<IActionResult> logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var roles = await _repository.getRoles();
            List<SelectListItem> RoleItems = roles.Select(role => new SelectListItem
            {
                Value = role.Id.ToString(),
                Text = role.Name
            }).ToList();
            ViewBag.roles = RoleItems;
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
                  
                    var user=await _repository.getUserByEmail(registerDetails.Email);
                    if(user.Email == null)
                    {
                        registerDetails.Password = await _queueService.HashPassword(registerDetails.Password);

                        var result = await _repository.createUser(registerDetails);
                        if (result)
                        {
                            //assign role
                            await _repository.AddUserToRole(registerDetails.Email, registerDetails.Role);
                            return RedirectToAction("Login");
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
        [Authorize(Roles = "ServiceProvider")]
        public IActionResult SelectServicePoint()
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
        public async Task<IActionResult> SelectServicePoint(CheckInViewModel service)
        {
            HttpContext.Session.SetInt32("ServicePointId", service.ServiceId);
            return RedirectToAction("ServicePoint", "Queue", new { servicePointId = service.ServiceId });
        }
       
        public IActionResult AccessDenied()
        {

            return View();
        }

    }
}