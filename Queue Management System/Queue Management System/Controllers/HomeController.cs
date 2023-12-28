using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;
using System.Diagnostics;

namespace Queue_Management_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //return View("/Views/");
            // return View("./Views/Home/Index.cshtml");
            string key = GenerateSecretKey();
            return View("~/Views/Home/Index.cshtml");


        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string GenerateSecretKey()
        {
            byte[] bytes = new byte[32]; // Adjust the byte length as needed for your key
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            string secretKey = Convert.ToBase64String(bytes);

            return secretKey; // Return the generated secret key
        }
    }
}