﻿using System.Collections.Generic;
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

        private readonly IServiceProviderService _serviceProviderService;

        public AccountController(IAuthenticationService authenticationService, IServiceProviderService serviceProviderService, ILogger<AccountController> logger)
        {
            _authenticationService = authenticationService;
            _serviceProviderService = serviceProviderService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            var serviceProviders = _serviceProviderService.GetServiceProvidersWithServicePoints();
            var viewModel = new LoginViewModel
            {
                AvailableServiceProviders = serviceProviders
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult LoginPost(LoginViewModel loginData)
        {
           
            _logger.LogInformation("username received is {0}", loginData.Username);
            _logger.LogInformation("password received is {0}", loginData.Password);

            var serviceProviders = _serviceProviderService.GetServiceProvidersWithServicePoints();
            
            var viewModel = new LoginViewModel
            {
                AvailableServiceProviders = serviceProviders
            };
            

            if (ModelState.IsValid)
            {
                if (_authenticationService.AuthenticateServiceProvider(loginData.Username, loginData.Password))
                {
                    _logger.LogInformation("Authentication success");

                    var currentServiceProvider = _authenticationService.GetServiceProviderByUsername(loginData.Username);
                    
                    var currentServiceProviderRole = currentServiceProvider.Role;

                    var currentServiceProviderId = currentServiceProvider.ServiceProviderId;

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, loginData.Username),
                        new Claim(ClaimTypes.Role, currentServiceProviderRole),
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties();

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    return RedirectToAction("ServicePoint", "Queue", new {currentServiceProviderId});
                }
                else
                {
                    _logger.LogInformation("Authentication fail");
                    ModelState.AddModelError("", "Invalid password, try again");
                    return View("login", viewModel);
                }
            }

            _logger.LogInformation("validation fail");

            return View("login", viewModel);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
