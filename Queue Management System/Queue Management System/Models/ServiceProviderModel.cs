namespace Queue_Management_System.Models
{
    public class ServiceProviderModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PasswordHash { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string ServicePoint { get; set; }
        public int ServiceTypeId { get; set; }
        public bool IsAuthorized { get; set; }
    }
}
