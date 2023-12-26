using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

using Queue_Management_System.Database;
using Queue_Management_System.Models;

namespace Queue_Management_System.Controllers
{
    public class ServicePointProviderController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly DatabaseConnection _databaseConnection;

        public ServicePointProviderController(IConfiguration configuration, DatabaseConnection databaseConnection)
        {
            _configuration = configuration;
            _databaseConnection = databaseConnection;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(ServicePointProvider servicePointProvider)
        {
            using (NpgsqlConnection connection = _databaseConnection.OpenConnection())
            {
                string sqlquery = "SELECT UserName FROM ServiceProvider WHERE UserName = @UserName AND Password = @Password";
                using (NpgsqlCommand command = new NpgsqlCommand(sqlquery, connection))
                {
                    command.Parameters.AddWithValue("@UserName", servicePointProvider.UserName);
                    command.Parameters.AddWithValue("@Password", servicePointProvider.Password);

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            HttpContext.Session.SetString("username", servicePointProvider.UserName);
                            return RedirectToAction("Welcome");
                        }
                        else
                        {
                            ViewData["Message"] = "User Login Details Failed!";
                            return View();
                        }
                    }
                }
            }
        }

        [HttpGet]
        public IActionResult Welcome()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
    }
}
