
//probably 1 to 1 relationship with ServicePoint
using Queue_Management_System.Models;

public class ServiceProvider {

    public int ServiceProviderId {get; set;}

   public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public int ServicePointId { get; set; }
    
     public string Role { get; set; }

    public virtual ServicePoint AssociatedServicePoint { get; set; }


     private string GenerateRandomPassword()
    {
        int length = 12;
        string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+";
        
        Random random = new Random();
        string randomPassword = new string(Enumerable.Repeat(characters, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());

        // maybe considering using bcrypt..

        return randomPassword;
    }


}