using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Services;
using Queue_Management_System.Repositories;

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
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string returnUrl = null)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            var serviceProvider = _authenticationService.AuthenticateUser(email, password);

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

            };

            await HttpContext.SignInAsync("MyAuthScheme", new ClaimsPrincipal(claimsIdentity), authProperties);
           
            //add user info to session
            var servicePoint = _servicePointRepository.GetServicePointByProviderId(serviceProvider.Id);

            if (servicePoint != null)
            {
                HttpContext.Session.SetString("servicePointId", servicePoint.Id);
                HttpContext.Session.SetString("serviceDescription", servicePoint.Description);
            }
           
            return LocalRedirect(returnUrl);

        }

    }
}
