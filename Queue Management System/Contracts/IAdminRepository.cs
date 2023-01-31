using Queue_Management_System.Models;

namespace Queue_Management_System.Contracts
{
    public interface IAdminRepository
    {
        Task<IEnumerable<AdminVM>> GetAll();

        Task<AdminVM> Get(int id);

        Task Add(AdminVM game);

        Task Update(int id, AdminVM game);

        Task Delete(int id);
    }
}
