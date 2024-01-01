
using Queue_Management_System.Models;

public interface IServiceProviderService {

   public List<ServiceProvider> GetServiceProviders();

   public ServiceProvider GetServiceProviderById(int serviceProviderId);

   public List<ServiceProvider> GetServiceProvidersWithServicePoints();

   public void AddServiceProviderWithServicePoints(ServiceProvider serviceProvider, List<int> servicePointIds);

   public ServiceProvider GetServiceProviderWithServicePointsById(int serviceProviderId);

   public void DeleteServiceProviderWithServicePoints(int serviceProviderId);

    public void UpdateServiceProviderWithServicePoints(ServiceProvider serviceProvider, List<int> newServicePointIds);

    public bool IsUsernameUnique(string username);








}