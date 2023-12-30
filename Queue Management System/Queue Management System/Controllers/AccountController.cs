using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Queue_Management_System.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthenticationService authenticationService, ILogger<AccountController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoginPost(LoginViewModel loginData)
        {
            if (ModelState.IsValid)
            {
                if (_authenticationService.AuthenticateServiceProvider(loginData.Username, loginData.Password))
                {
                    _logger.LogInformation("Authentication success");

                    var userRole = _authenticationService.GetServiceProviderByUsername(loginData.Username).Role;

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, loginData.Username),
                        new Claim(ClaimTypes.Role, userRole),
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties();

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    return RedirectToAction("ServicePoint", "Queue");
                }
                else
                {
                    _logger.LogInformation("Authentication fail");
                    ModelState.AddModelError("", "Invalid username or password, try again");
                    return View("Login", loginData);
                }
            }

            return View("Login", loginData);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
