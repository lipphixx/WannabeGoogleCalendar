using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using Microsoft.EntityFrameworkCore;

namespace WannabeGCalendar.Entities
{
    [Table("Events")]
    public class Event
    {
        [Key]
        [Column("EventID")]
        public int EventId { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public TimeSpan? EventTime { get; set; }
        public string? EventNote  { get; set; }
        [Column("OwnerID")]
        public int OwnerId { get; set; }
        public string? Participants { get; set; }
    }
}
