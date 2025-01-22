using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WannabeGCalendar.Entities
{
    [Table("Users")]
    public class Users
    {
        [Key]
        [Column("UserID")]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

    }
}
