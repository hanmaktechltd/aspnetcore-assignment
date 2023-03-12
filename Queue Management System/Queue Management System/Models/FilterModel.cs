namespace Queue_Management_System.Models
{
    public class FilterModel
    {
        public int Id { get; set; }
        public int ServicePointId { get; set; }
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; }

    }
}
