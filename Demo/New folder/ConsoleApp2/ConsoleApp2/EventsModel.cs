

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoSatva.Model
{
    [Table("Events")]
    public class EventsModel
    {
        public int Id { get; set; }
        [Required]
        public string EventTitle { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public string EventDescription { get; set; }
        [Range(int.MinValue, int.MaxValue)]
        public int EventPriority { get; set; }
    }
}

