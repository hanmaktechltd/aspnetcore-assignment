namespace Queue_Management_System.ServiceInterface
{
    public interface IServicePointOperations
    {
        Task<bool> MarkFinishedAndInsert(string ticketNumber, int serviceProviderId);
        Task<bool> MarkAsNoShow(string ticketNumber);

        Task<bool> UpdateRecallCount(string ticketNumber, int recallCount);
    }
}
