namespace Queue_Management_System.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int CustomerNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public ServicePointEnum ServicePointEnum  { get; set; }
        public  ServiceProviderEnum ServiceProviderEnum { get; set; }
        public int WaitingTimeInMinutes { get; set; }
    }
}
