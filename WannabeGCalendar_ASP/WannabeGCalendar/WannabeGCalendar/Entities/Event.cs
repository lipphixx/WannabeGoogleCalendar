using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public DateOnly EventDate { get; set; }
        public TimeOnly EventTime { get; set; }
        public string? EventNote  { get; set; }
        [Column("OwnerID")]
        public int OwnerId { get; set; }
        [Column("ParticipantID")]
        public int? ParticipantId { get; set; }
    }
}
