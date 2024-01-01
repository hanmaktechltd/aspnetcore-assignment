using Queue_Management_System.Models;

public class ServiceProvider
{
    public int ServiceProviderId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public List<ServicePoint> ServicePoints { get; set; }

    public ServiceProvider()
    {
        Password = SimplePasswordGenerator();
    }

    private string SimplePasswordGenerator()
    {
        const int length = 12;
        const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+";

        Random random = new Random();
        string randomPassword = new string(Enumerable.Repeat(characters, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());

        // Consider using bcrypt for password hashing

        return randomPassword;
    }

}
