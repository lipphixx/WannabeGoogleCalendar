using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WannabeGCalendar.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        [Column("UserID")]
        public int? UserId { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }

        public User(int? userId, string email,string? password, string username, string fullName, string phoneNumber)
        {
            UserId = userId;
            Email = email;
            Password = password;
            Username = username;
            FullName = fullName;
            PhoneNumber = phoneNumber;
        }
    }
}
