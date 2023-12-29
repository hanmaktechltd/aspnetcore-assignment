using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;
using System.Security.Claims;
using Npgsql;

namespace Queue_Management_System.Controllers
{
    public class AccountController : Controller
    {
        private const string _tableName = "AppUsersTb";
        private IConfiguration _config;
        public AccountController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Login(string ReturnUrl ="")
        {
            LoginM loginModel = new LoginM();

            loginModel.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginM loginModel)
        {
            if (ModelState.IsValid)
            {
                string userAuthenticationQuery = $"SELECT name, role, servicepointid " +
                                                $"FROM {_tableName} " +
                                                $"WHERE Name='{loginModel.Name}' " +
                                                $"AND Password='{loginModel.Password}'";
                UserM appUser = AuthenticateAppUser(userAuthenticationQuery);
                if (appUser == null)
                {
                    ViewBag.Message = "Name or Password is incorrect!";
                    return View(loginModel);
                }
                else
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, loginModel.Name),
                        new Claim(ClaimTypes.Role, appUser.Role),
                        new Claim("ServicePointId", Convert.ToString(appUser.ServicePointId))
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                      new ClaimsPrincipal(claimsIdentity));

                     if (loginModel.ReturnUrl == "/")
                     {
                         return Redirect(loginModel.ReturnUrl);
                     }
                    else if (appUser.Role == "Admin")
                     {
                         return RedirectToAction("Dashboard", "Admin");
                     }
                     else if (appUser.Role == "Service Provider")
                     {
                         return RedirectToAction("ServicePoint", "Queue");
                     }
                    
                }
            }
            //return RedirectToAction("ServicePoints", "Admin");
            return View(loginModel);
        }
        public UserM? AuthenticateAppUser(string userAuthenticationQuery)
        {
            UserM appUser = null;
            string connectionString = _config.GetConnectionString("DefaultConnection");
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                // Prep command object.
                NpgsqlCommand command = new NpgsqlCommand(userAuthenticationQuery, connection);
                connection.Open();
                // Obtain a data reader via ExecuteReader()
                using (NpgsqlDataReader dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        appUser = new UserM
                        {
                            Name = dataReader["name"].ToString(),
                            Role = dataReader["role"].ToString(),
                            ServicePointId = Convert.ToInt32(dataReader["servicepointid"])
                        };
                    }
                   
                    dataReader.Close();
                }
                connection.Close();

            }
            
            if (appUser == null)
            {
                return null;
            }
            return appUser;
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect("/");
        }

    }
}
