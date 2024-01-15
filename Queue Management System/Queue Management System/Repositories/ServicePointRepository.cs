using Queue_Management_System.Models;

namespace Queue_Management_System.Repositories
{
    public interface IServicePointRepository
    {
        ServicePointModel GetServicePointById(string id);

        IEnumerable<ServicePointModel> GetServicePoints();

        void AddServicePoint(ServicePointModel servicePoint);

        void UpdateServicePoint(ServicePointModel servicePoint);

        void DeleteServicePoint(ServicePointModel servicePoint);
    }
}