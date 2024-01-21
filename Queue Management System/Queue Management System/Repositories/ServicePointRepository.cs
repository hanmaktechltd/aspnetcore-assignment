using Queue_Management_System.Models;

namespace Queue_Management_System.Repositories
{
    public interface IServicePointRepository
    {
        ServicePointModel GetServicePointById(string id);

        ServicePointModel GetServicePointByProviderId(string serviceProviderId);

        IEnumerable<ServicePointModel> GetServicePoints();

        void AddServicePoint(ServicePointModel servicePoint);

        void UpdateServicePoint(ServicePointModel servicePoint);

        void DeleteServicePoint(string id);
    }
}