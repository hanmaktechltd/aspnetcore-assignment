using Queue_Management_System.Data;
using System.Net;

namespace Queue_Management_System.Models
{
    public class AdminVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
       /* public List<ServicePointVM> ServicePoints { get; set; }*/
/*        public List<ServiceProviderVM> ServiceProviders { get; set; }*/
       
    }
}
