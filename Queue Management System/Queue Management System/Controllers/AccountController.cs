using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Services;
using Queue_Management_System.Repositories;
using Queue_Management_System.Models;

namespace Queue_Management_System.Controllers
{
    public class AccountController : Controller
    {
        private readonly Services.IAuthenticationService _authenticationService;

        private readonly IServicePointRepository _servicePointRepository;

        public AccountController(Services.IAuthenticationService authenticationService, IServicePointRepository servicePointRepository)
        {
            _authenticationService = authenticationService;
            _servicePointRepository = servicePointRepository;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel user, string returnUrl)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            var serviceProvider = _authenticationService.AuthenticateUser(user.Email, user.Password);

            if (serviceProvider == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, serviceProvider.Id),
                new Claim(ClaimTypes.Name, serviceProvider.Name),
                new Claim(ClaimTypes.Email, serviceProvider.Email),
            };

            var claimsIdentity = new ClaimsIdentity(claims, "MyAuthScheme");
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(1)
            };

            await HttpContext.SignInAsync("MyAuthScheme", new ClaimsPrincipal(claimsIdentity), authProperties);
           
            //add user info to session
            var servicePoint = await _servicePointRepository.GetServicePointByServiceProviderId(serviceProvider.Id);

            if (servicePoint != null)
            {
                HttpContext.Session.SetString("servicePointId", servicePoint.Id);
                HttpContext.Session.SetString("serviceDescription", servicePoint.Description);
            }
           
            return LocalRedirect(returnUrl);

        }

    }
}
