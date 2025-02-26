using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WannabeGCalendar.Entities;

[Table("PasswordRecovery")]
public class Recovery
{
    [Key]
    public int RecId { get; set; }
    public string RecoveryEmail { get; set; }
    public string RecoveryCode { get; set; }
    public DateTime ExpiresAt { get; set; }
}