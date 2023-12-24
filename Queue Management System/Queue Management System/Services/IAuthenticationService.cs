 
 public interface IAuthenticationService {

    ServiceProvider GetServiceProviderByUsername(string username);

    bool AuthenticateServiceProvider(string username, string providedPassword);




 }

