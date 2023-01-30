using System.ComponentModel.DataAnnotations.Schema;

namespace Queue_Management_System.Data
{
    public class QueList
    {
        public int Id { get; set; }

        [ForeignKey("ServicePointId")]
        public ServicePoint ServicePoint { get; set; }
        public int ServicePointId { get; set; }
    }
}
