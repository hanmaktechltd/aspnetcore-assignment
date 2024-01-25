namespace Queue_Management_System.Models
{
    public class ServicePointAnalyticModel
    {
        public string ServicePointId {get; set;}

        public TimeSpan AverageWaitingTime{get; set;}

        public TimeSpan AverageServiceTime{get; set;}

        public Int32 TotalCustomers{get; set;}

    }
}