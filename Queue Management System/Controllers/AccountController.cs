using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Queue_Management_System.Common;
using Queue_Management_System.Data;
using Queue_Management_System.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Queue_Management_System.Controllers
{
    public class AccountController : Controller
    {
        private const string _tableName = "users";
        private IConfiguration _config;
        CommonHelper _helper;
        public AccountController(IConfiguration config)
        {
            _config = config;
            _helper = new CommonHelper(_config);
        }
        public IActionResult Login(string ReturnUrl = "/")
        {
            LoginVM objLoginModel = new LoginVM();
            objLoginModel.ReturnUrl = ReturnUrl;
            return View(objLoginModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (string.IsNullOrEmpty(vm.Name) && string.IsNullOrEmpty(vm.Password))
            {
                ViewBag.ErrorMsg = "Name and Password Empty";
                return View();
            }
            else
            {

                
                bool Isfind = SignInMethod(vm.Name, vm.Password);
                var getUserRole = HttpContext.Session.GetString("Role");
                var getUserServingPointId = HttpContext.Session.GetInt32("ServicePointId");
                if (Isfind == true)
                {
                    ViewBag.Success = "Thanks for Login";
                    var claims = new List<Claim>
                            {
                              new Claim(ClaimTypes.NameIdentifier,Convert.ToString(getUserServingPointId)),
                              new Claim(ClaimTypes.Name, vm.Name),
                              new Claim(ClaimTypes.Role, getUserRole)

                            };
                    /*  var claimsIdentity = new ClaimsIdentity(claims, "Login");
                      await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));*/

                    //Initialize a new instance of the ClaimsIdentity with the claims and authentication scheme
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    //Initialize a new instance of the ClaimsPrincipal with ClaimsIdentity
                    var principal = new ClaimsPrincipal(identity);
                    //SignInAsync is a Extension method for Sign in a principal for the specified scheme.
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        principal, new AuthenticationProperties() { IsPersistent = vm.RememberLogin });

                    if (vm.ReturnUrl == "/Admin/Dashboard")
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else if ((vm.ReturnUrl == "/Queue/ServicePoint"))
                    {
                        return RedirectToAction("ServicePoint", "Queue");
                    }
                }
            }
            var getUserServingPointId2 = HttpContext.Session.GetInt32("ServicePointId"); //not implemented
            var role = HttpContext.Session.GetString("Role");
            //Add if condition according to role
            if (role == "Admin")
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                return RedirectToAction("ServicePoint", "Queue");
            }
            
        }
        private bool SignInMethod(string name, string password)
        {
            bool flag = false;
            string query = $"select * from {_tableName} where Name='{name}' and Password='{password}'  ";
            var userDetails = _helper.GetUserByUserName(query);

            if (userDetails.Name != null)
            {
                flag = true;

                HttpContext.Session.SetString("Name", userDetails.Name);
                HttpContext.Session.SetString("Role", userDetails.Role);
                HttpContext.Session.SetInt32("ServicePointId", userDetails.ServicePointId);

            }
            else
            {
                ViewBag.ErrorMsg = "Name $ Password wrong";
            }
            return flag;
        }



        public async Task<IActionResult> Logout()
        {
            //SignOutAsync is Extension method for SignOut
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //Redirect to home page
            return LocalRedirect("/");

        }
    }



}
