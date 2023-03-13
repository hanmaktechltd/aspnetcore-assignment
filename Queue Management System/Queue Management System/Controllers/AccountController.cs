using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Core.Types;
using Queue_Management_System.Data;
using Queue_Management_System.Models.ViewModels;
using Queue_Management_System.Repository;
using System.Data;
using System.Security.Claims;

namespace Queue_Management_System.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly QueueDBContext _context;
        private readonly string conString;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly QueueRepository _repository;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            QueueDBContext context,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager,
            QueueRepository repository
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            conString = configuration.GetConnectionString("DefaultConnection");
            _roleManager = roleManager;
            _repository = repository;
        }

        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginDetails, string? returnUrl=null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _signInManager.PasswordSignInAsync(loginDetails.Email, loginDetails.Password, loginDetails.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        var user= _userManager.Users.SingleOrDefault(r => r.Email == loginDetails.Email);
                        var roles = _userManager.GetRolesAsync(user);
                        var claims = await _userManager.GetClaimsAsync(user);
                        foreach (var role in roles.Result)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }

                        //Initialize a new instance of the ClaimsIdentity with the claims and authentication scheme
                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        //Initialize a new instance of the ClaimsPrincipal with ClaimsIdentity
                        var principal = new ClaimsPrincipal(identity);
                        //SignInAsync is a Extension method for Sign in a principal for the specified scheme.
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                            principal, new AuthenticationProperties() { IsPersistent = loginDetails.RememberMe });
                        if(await _userManager.IsInRoleAsync(user, "Admin"))
                        {
                            return RedirectToAction("Dashboard", "Admin");
                        }
                        if (await _userManager.IsInRoleAsync(user, "ServiceProvider"))
                        {
                            return RedirectToAction("SelectServicePoint", "Account");
                        }
                        if (returnUrl == null)
                        {
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
                var dsfsd = ex.Message;
                return BadRequest(ModelState);
            }
        }

        public async Task<IActionResult> logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
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
                        await  AddUserToRole(registerDetails);
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
        [Authorize(Roles = "ServiceProvider")]
        public IActionResult SelectServicePoint()
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
        public async Task<IActionResult> SelectServicePoint(CheckInViewModel service)
        {
            HttpContext.Session.SetInt32("ServicePointId", service.ServiceId);
            return RedirectToAction("ServicePoint", "Queue", new { servicePointId = service.ServiceId });
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
        public IActionResult AccessDenied()
        {
           
            return View();
        }

    }
}
