using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Repositories;
using Queue_Management_System.Models;

namespace Queue_Management_System.Controllers
{
    public class QueueController : Controller
    {
        private readonly IServiceRepository _serviceRepository;

        public QueueController(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        [HttpGet]
        public IActionResult CheckinPage(string serviceId)
        {

            Console.WriteLine(serviceId);

            var services = _serviceRepository.GetServices();
            var viewModel = new ServicesViewModel(){
                Services = services.ToList().ToArray() // refactor
            };

            return View(viewModel);
        }



        [HttpGet]
        public IActionResult WaitingPage()
        {
            return View();
        }



        [Authorize, HttpGet]
        public IActionResult ServicePoint()
        {
            return View();
        }


    }
}
