using System.Collections.Generic;
using System.Threading.Tasks;

namespace Queue_Management_System.Models
{
    public interface IRepository
    {
        Task<List<ServiceTypeModel>> GetAvailableServicesAsync();
    }
}
