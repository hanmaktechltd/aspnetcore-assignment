namespace Queue_Management_System.Models
{
    public class AverageServiceTimePerServicePoint
    {
        public int ServicePointId { get; set; }
        public TimeSpan AverageServiceTime { get; set; }
    }

}
