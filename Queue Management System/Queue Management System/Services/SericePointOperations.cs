using Queue_Management_System.Repository;
using Queue_Management_System.ServiceInterface;
using System;
using System.Threading.Tasks;

namespace Queue_Management_System.Services
{
    public class ServicePointOperations : IServicePointOperations
    {
        private readonly DbOperationsRepository _dbOperationsRepository;

        public ServicePointOperations(DbOperationsRepository dbOperationsRepository)
        {
            _dbOperationsRepository = dbOperationsRepository;
        }

        public async Task<bool> MarkFinishedAndInsert(string ticketNumber, int serviceProviderId)
        {
            try
            {
                if (await _dbOperationsRepository.InsertIntoFinishedTableAsync(ticketNumber, serviceProviderId))
                {
                    return await _dbOperationsRepository.UpdateQueueEntryMarkFinishedAsync(ticketNumber);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MarkFinishedAndInsert: {ex.Message}");
            }

            return false;
        }

        public async Task<bool> MarkAsNoShow(string ticketNumber)
        {
            try
            {
                return await _dbOperationsRepository.UpdateNoShowStatus(ticketNumber);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MarkAsNoShow: {ex.Message}");
            }

            return false;
        }
        public async Task<bool> UpdateRecallCount(string ticketNumber, int recallCount)
        {
            try
            {
                return await _dbOperationsRepository.UpdateRecallCount(ticketNumber, recallCount);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MarkAsNoShow: {ex.Message}");
            }
            return false;
        }

    }
}
