using Npgsql;
using Queue_Management_System.Models;

namespace Queue_Management_System.Services
{
    public interface IServicePointRepository
    {
        Task<List<CustomerTicket>> GetQueueStatus(int? servicePointId);
        Task<CustomerTicket> GetNextNumber(int? servicePointId);
        Task<CustomerTicket> RecallNumber(int Id, int? servicePointId);
        Task MarkAsNoShow(int Id, int? servicePointId);
        Task MarkAsFinished(int Id);
        Task<TransferView> TransferNumber(int Id, int? servicePointId);
        Task TransferPost(int Id, int servicePointId);
        Task<Models.ServiceProvider> Login(string Name, string Password);
        Task<Models.ServiceProvider> AuthenticateProvider(string query, List<NpgsqlParameter> parameters);
    }
}
