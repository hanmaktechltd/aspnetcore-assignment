using Queue_Management_System.Models;

namespace Queue_Management_System.ViewModels
{
    public class ServiceProviderViewModel
    {
        public IEnumerable<ServiceProviderModel> ServiceProviderList { get; set; }
        public ServiceProviderModel SingleServiceProvider { get; set; }
    }
}
