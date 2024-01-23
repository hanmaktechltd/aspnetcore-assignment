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

    public class MockServicePointRepository : IServicePointRepository
    {
        public ServicePointModel GetServicePointById(string id)
        {
            return new ServicePointModel(){
                Id = id,
                Description = "Mock Service Point for GetServicePointById Method",
                ServiceProviderId = null
            };
        }

        public ServicePointModel GetServicePointByProviderId(string serviceProviderId)
        {
            return new ServicePointModel(){
                Id = "MockID_ServicePoint_009",
                Description = "Mock Service Point for GetServicePointByProviderId Method",
                ServiceProviderId = serviceProviderId
            }; 
        }

        public IEnumerable<ServicePointModel> GetServicePoints()
        {
            ServicePointModel SamplePoint1 = new ServicePointModel(){
                Id = "SP_001",
                Description = "Mock Service Point 1",
                ServiceProviderId = null
            };

            ServicePointModel SamplePoint2 = new ServicePointModel(){
                Id = "SP_002",
                Description = "Mock Service Point 2",
                ServiceProviderId = null
            };

            ServicePointModel SamplePoint3 = new ServicePointModel(){
                Id = "SP_003",
                Description = "Mock Service Point 3",
                ServiceProviderId = null
            };

            return new List<ServicePointModel> (){
                SamplePoint1, SamplePoint2, SamplePoint3
            };
        }

        public void AddServicePoint(ServicePointModel servicePoint)
        {

        }

        public void UpdateServicePoint(ServicePointModel servicePoint)
        {

        }

        public void DeleteServicePoint(string id)
        {

        }
    }

}