﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Queue_Management_System.Common;
using Queue_Management_System.Data;
using Queue_Management_System.Models;
using System.Security.Claims;

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
                var gerUserrole = HttpContext.Session.GetString("Role");
                if (Isfind == true)
                {
                    ViewBag.Success = "Thanks for Login";
                    var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, vm.Name),
                                new Claim(ClaimTypes.Role, gerUserrole)
                            };
                    var claimsIdentity = new ClaimsIdentity(claims, "Login");
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

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
