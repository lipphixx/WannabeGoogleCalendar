using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WannabeGCalendar.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        [Column("UserID")]
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
