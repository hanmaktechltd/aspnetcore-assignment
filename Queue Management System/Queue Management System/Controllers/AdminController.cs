using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;
using Queue_Management_System.Repository;
using Queue_Management_System.ServiceInterface;

namespace Queue_Management_System.Controllers
{
   // [Authorize]
    public class AdminController : Controller
    {

        private readonly DbOperationsRepository _dbOperationsRepository;

        public AdminController(DbOperationsRepository dbOperationsRepository)
        {
            _dbOperationsRepository = dbOperationsRepository;
        }

       
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var finishedEntries = await _dbOperationsRepository.GetFinishedEntries();
                return View(finishedEntries);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while fetching data.");
                return View(new List<ServedCustomers>()); 
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult ServicePoints()
        {
            return View();
        }
        public IActionResult ServiceProviders()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Admin model)
        {
            if (ModelState.IsValid)
            {
                var user = await _dbOperationsRepository.AdminLoginAsync(model.UsernameOrEmail, model.Password); 
                if (user != null)
                {
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt");
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddServiceProvider(ServiceProviderModel model)
        {
            if (ModelState.IsValid)
            {
                // Perform validation and other checks if required

                bool isServiceProviderAdded = await _dbOperationsRepository.InsertServiceProviderAsync(model);

                if (isServiceProviderAdded)
                {
                    // Redirect to the appropriate action or return a success message
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to add service provider.");
                    return View(model); // Return the view with error messages
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddServiceType(ServiceTypeModel model)
        {
            if (ModelState.IsValid)
            {
                // Perform validation and other checks if required

                bool isServiceTypeAdded = await _dbOperationsRepository.InsertServiceTypeAsync(model);

                if (isServiceTypeAdded)
                {
                    // Redirect to the appropriate action or return a success message
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to add service type.");
                    return View(model); // Return the view with error messages
                }
            }

            return View(model);
        }

    }
}
