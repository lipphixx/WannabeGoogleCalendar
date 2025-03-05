using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WannabeGCalendar.Entities;

[Table("UsersLabels")]
public class UserLabel
{
    [Key]
    public int UserId { get; set; }
    public string Labels { get; set; }
}