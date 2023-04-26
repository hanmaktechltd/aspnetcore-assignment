using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;

namespace Queue_Management_System.Services
{
    public interface ICheckInRepository
    {
        Task<FileStreamResult> CheckIn(int servicePointId, int serviceProviderId);

        Task<List<CustomerTicket>> Waiting();
    }
}
